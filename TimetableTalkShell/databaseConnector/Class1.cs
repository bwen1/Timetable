using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace databaseConnector
{
    public enum statuscode { OK, ERROR, NOT_THESE_DROIDS, INVALID_DATA }
    public enum day { Mon, Tue, Wed, Thur, Fri, Sat, Sun }
    public enum friends { NO, PENDING_TO, PENDING_FROM, BLOCKED_TO, BLOCKED_FROM, YES }
    public struct Response
    {
        public statuscode status;
        public string error { get; private set; }
        public string message { get; private set; }
        public Response(statuscode statuscode, string message)
        {
            status = statuscode;
            this.message = message;
            error = null;
        }
        public Response(statuscode statuscode, string message, string error)
        {
            status = statuscode;
            this.message = message;
            this.error = error;
        }

    }
    public struct Event
    {
        public int ID { get; private set; }
        public int eID { get; private set; }
        public bool shared { get; private set; }
        public string eventName { get; private set; }
        public string location { get; private set; }
        public string startTime { get; private set; }
        public string endTime { get; private set; }
        public day Day { get; private set; }
        public string notes { get; private set; }

        public Event(int eventID, int userID, string eventName, bool shared, string startTime, string endTime, day d)
        {
            this.ID = userID;
            this.eID = eventID;
            this.eventName = eventName;
            this.shared = shared;
            this.startTime = startTime;
            this.endTime = endTime;
            this.Day = d;
            location = null;
            notes = null;
        }
        public Event(int eventID, int userID, string eventName, bool shared, string startTime, string endTime, day d, string location, string notes)
        {
            this.ID = userID;
            this.eID = eventID;
            this.eventName = eventName;
            this.shared = shared;
            this.startTime = startTime;
            this.endTime = endTime;
            this.Day = d;
            this.location = location;
            this.notes = notes;
        }

        public Event(string eventName, bool shared, string startTime, string endTime, day d, string location, string notes = "")
        {
            this.ID = 0;
            this.eID = 0;
            this.eventName = eventName;
            this.shared = shared;
            this.startTime = startTime;
            this.endTime = endTime;
            this.Day = d;
            this.location = location;
            this.notes = notes;
        }
    }
    public struct User
    {
        public int ID;
        public string username;
        public friends friend;
        public User(int ID, string username, friends friend)
        {
            this.ID = ID;
            this.username = username;
            this.friend = friend;
        }
    }
    public class Backend
    {
        private int UID;
        public string UserName { get; private set; }
        private static readonly HttpClient client = new HttpClient();
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private List<string> takenNames;
        private List<User> friendarray;
        private List<Event> events;
        private List<Event> myevents;
        public Backend()
        {
            takenNames = new List<string>();
            takenNames.Add("");
            friendarray = new List<User>();
            events = new List<Event>();
            myevents = new List<Event>();
            UID = 0;
        }

        public bool OpenConnection()
        {
            return true;
        }
        public bool CloseConnection()
        {
            return true;
        }

        /// <summary>
        /// Is a user currently logged in, does not refer to database connectivity.
        /// </summary>
        /// <returns>the statments truth</returns>
        public bool IsLoggedIn()
        {
            return (UID == 0 ? false : true);
        }

        /// <summary>
        /// Queries the database to see if that username is taken yet.
        /// </summary>
        /// <param name="username">the username to search for</param>
        /// <returns>if the username is currently not in use</returns>
        public async Task<bool> IsUsernameAvalible(string username)
        {
            bool Avalible;
            if (takenNames.Contains(username))
                return false;

            string responseString = await client.GetStringAsync("http://ec2-3-82-249-155.compute-1.amazonaws.com:3000/users/avcheck/" + username);
            //Read the data determine the result
            dynamic obj = JsonConvert.DeserializeObject(responseString);
            if (!obj.Error)
            {
                if (obj.Message == "Name taken")
                {
                    Avalible = false;
                    takenNames.Add(username);
                }
                else
                {
                    Avalible = true;
                }
            }
            else
            {
                Avalible = false;
            }
            return Avalible;
        }

        #region sha256 string generator, from https://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
        private static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// Logs the user in, actually It queries the database to confirm details, then sets the users ID as the active user.
        /// </summary>
        /// <param name="username"> the users username</param>
        /// <param name="password"> the users password, this is not ever stored anyware</param>
        /// <returns> A response</returns>
        public async Task<Response> LogIn(string username, string password)
        {
            bool Avalible;
            string psh = GetHashString(password);
            Console.WriteLine(psh);
            var values = new Dictionary<string, string>
            {
                { "username", username },
                { "password", psh }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("http://ec2-3-82-249-155.compute-1.amazonaws.com:3000/users/login", content);

            var responseString = await response.Content.ReadAsStringAsync();
            //Read the data determine the result
            dynamic obj = JsonConvert.DeserializeObject(responseString);

            if (!obj.Error)
            {
                if (obj.Message == "Login sucess")
                {
                    Avalible = true;
                    UID = obj.id;
                    UserName = username;
                }
                else
                    Avalible = false;
            }
            else
                Avalible = false;
            if (Avalible)
                return new Response(statuscode.OK, UID.ToString());
            if(obj.Error)
                return new Response(statuscode.ERROR, "Could not open sql database");
            return new Response(statuscode.NOT_THESE_DROIDS, "Username or password was incorrect");
        }

        /// <summary>
        /// Logs the user out, actually un-asgines the current user value.
        /// </summary>
        /// <returns>A response, not really useful</returns>
        public Response LogOut()
        {
            UID = 0;
            UserName = "";
            friendarray = new List<User>();
            events = new List<Event>();
            myevents = new List<Event>();
            return new Response(statuscode.OK, "Nothing to see here!");
        }

        /// <summary>
        /// Registers the user in the database, you should probobly check if the username is avalible first.
        /// note: this does not log in the new user.
        /// </summary>
        /// <param name="username">the desired username</param>
        /// <param name="password">the desired password, (not stored)</param>
        /// <returns>OK if its all good, or error if the user already exists</returns>
        public async Task<Response> SignUp(string username, string password)
        {
            if (await IsUsernameAvalible(username))
            {
                bool Avalible;
                string psh = GetHashString(password);
                Console.WriteLine(psh);
                var values = new Dictionary<string, string>
            {
                { "username", username },
                { "password", psh }
            };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("http://ec2-3-82-249-155.compute-1.amazonaws.com:3000/users/signup", content);

                var responseString = await response.Content.ReadAsStringAsync();
                //Read the data determine the result
                dynamic obj = JsonConvert.DeserializeObject(responseString);

                if (!obj.Error)
                {
                    if (obj.Message == "User registered")
                        Avalible = true;
                    else
                        Avalible = false;
                }
                else
                    Avalible = false;
                if (Avalible)
                    return new Response(statuscode.OK, "User registered sucessfully");
                if (obj.Error)
                    return new Response(statuscode.ERROR, "Could not open sql database");
                return new Response(statuscode.NOT_THESE_DROIDS, "Username or password was unavalible");
            }
            else
            {
                return new Response(statuscode.NOT_THESE_DROIDS, "Username is not avalible");
            }

        }

        /// <summary>
        /// Changes the current users username, to an avalible username.
        /// </summary>
        /// <param name="newUsername">the new username to give</param>
        /// <returns>a response</returns>
        public async Task<Response> ChangeUsername(string newUsername)
        {
            if (IsLoggedIn())
            {
                if (await IsUsernameAvalible(newUsername))
                {
                    var values = new Dictionary<string, string>
            {
                { "id", UID.ToString() },
                { "newname", newUsername }
            };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("http://ec2-3-82-249-155.compute-1.amazonaws.com:3000/users/changename", content);

                    var responseString = await response.Content.ReadAsStringAsync();
                    //Read the data determine the result
                    dynamic obj = JsonConvert.DeserializeObject(responseString);
                    if (!obj.error)
                    {
                        this.UserName = newUsername;
                        return new Response(statuscode.OK, "Username Updated");
                    }
                    return new Response(statuscode.ERROR, "Could not open database");
                }
                return new Response(statuscode.ERROR, "That username is not avalible");
            }
            return new Response(statuscode.ERROR, "user not logged in");
        }

        /// <summary>
        /// Changes the current users password to a new one, requires the user to verify their password to do so.
        /// </summary>
        /// <param name="newPassword">the new password</param>
        /// <param name="oldPassword">the old password</param>
        /// <returns>a response displaying the result</returns>
        public async Task<Response> ChangePassword(string newPassword, string oldPassword)
        {
            if (IsLoggedIn())
            {
                Response r = await LogIn(UserName, oldPassword);
                if (r.status == statuscode.OK)
                {
                    string newHash = GetHashString(newPassword);
                    var values = new Dictionary<string, string>
            {
                { "id", UID.ToString() },
                { "newpass", newPassword }
            };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("http://ec2-3-82-249-155.compute-1.amazonaws.com:3000/users/changepassword", content);

                    var responseString = await response.Content.ReadAsStringAsync();
                    //Read the data determine the result
                    dynamic obj = JsonConvert.DeserializeObject(responseString);
                    if (!obj.Error)
                    {
                        return new Response(statuscode.OK, "Password Updated");
                    }
                    return new Response(statuscode.ERROR, "Could not open database");
                }
                return new Response(statuscode.NOT_THESE_DROIDS, "Invalid current password");
            }
            return new Response(statuscode.ERROR, "user not logged in");
        }

        /// <summary>
        /// Sends a friend request to the selected user.
        /// </summary>
        /// <param name="user">the user to send to</param>
        /// <returns>OK if all good, Error if blocked, already a friend, or the user dosen't exist</returns>
        public async Task<Response> RequestFriend(User user)
        {
            if (IsLoggedIn())
            {
                if (!await IsUsernameAvalible(user.username))
                {
                    var values = new Dictionary<string, string>
            {
                { "id", UID.ToString() },
                { "friendid", user.ID.ToString() }
            };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("http://ec2-3-82-249-155.compute-1.amazonaws.com:3000/friends/friendrequest", content);

                    var responseString = await response.Content.ReadAsStringAsync();
                    //Read the data determine the result
                    dynamic obj = JsonConvert.DeserializeObject(responseString);
                    if (!obj.Error)
                    {
                        friendarray.Add(user);
                        return new Response(statuscode.OK, "Friend request sent sucessfully");
                    }
                    return new Response(statuscode.ERROR, "Could not open database");
                }
                else
                {
                    return new Response(statuscode.NOT_THESE_DROIDS, "That user does not exist");
                }
            }
            else
            {
                return new Response(statuscode.ERROR, "User is not logged in!");
            }
        }

        /// <summary>
        /// sends a friend request to the selected user.
        /// </summary>
        /// <param name="username"> the user to send to</param>
        /// <returns>If the user was sucessfully requested</returns>
        public async Task<Response> RequestFriend(string username)
        {
            if (IsLoggedIn())
            {
                return await RequestFriend(GetUser(username));
            }
            return new Response(statuscode.ERROR, "User not logged in");
        }

        /// <summary>
        /// sends a friend request to the selected user.
        /// </summary>
        /// <param name="userID"> the ID of the user to send to</param>
        /// <returns>If the user was sucessfully requested</returns>
        public async Task<Response> RequestFriend(int userID)
        {
            if (IsLoggedIn())
            {
                return await RequestFriend(new User(userID, "", friends.NO));
            }
            return new Response(statuscode.ERROR, "User not logged in");
        }

        /// <summary>
        /// The back backend function which updates friend requests, not sure if it works properly...
        /// </summary>
        /// <param name="newStatus">the stats to set the friend to</param>
        /// <param name="friendID">the friend in question</param>
        /// <param name="to">which direction was the request sent?</param>
        /// <returns>a response</returns>
        private async Task<Response> UpdateFriendRequestStatus(friends newStatus, int friendID, bool to)
        {
            if (IsLoggedIn())
            {
                int index = 0;
                foreach (User fri in friendarray)
                {
                    if (fri.ID == friendID)
                    {
                        if (newStatus == friends.NO)
                            return await RemoveFriend(friendID);
                        var values = new Dictionary<string, string>
            {
                { "id", UID.ToString() },
                { "to", to.ToString() },
                { "newstatus", newStatus.ToString() }
            };

                        var content = new FormUrlEncodedContent(values);

                        var response = await client.PostAsync("http://ec2-3-82-249-155.compute-1.amazonaws.com:3000/friends/updaterequest", content);

                        var responseString = await response.Content.ReadAsStringAsync();
                        //Read the data determine the result
                        dynamic obj = JsonConvert.DeserializeObject(responseString);
                        if (!obj.Error)
                        {
                            friendarray[index] = new User(friendarray[index].ID, friendarray[index].username, newStatus);

                            return new Response(statuscode.OK, "Friend request Updated sucessfully");
                        }
                        return new Response(statuscode.ERROR, "Could not open database");

                    }
                    index++;
                }
                return new Response(statuscode.NOT_THESE_DROIDS, "You don't have a friend with that ID");
            }
            return new Response(statuscode.ERROR, "User not logged in");
        }

        /// <summary>
        /// Accepts a pending friend request.
        /// </summary>
        /// <param name="friendID">the ID of the request to accept</param>
        /// <returns> if the acceptance was sucessful</returns>
        public async Task<Response> AcceptFriend(int friendID)
        {
            foreach (User user in friendarray)
            {
                if (user.ID == friendID)
                {
                    if (user.friend == friends.PENDING_TO)
                    {
                        return await UpdateFriendRequestStatus(friends.YES, friendID, true);
                    }
                    else if (user.friend == friends.PENDING_FROM)
                    {
                        return new Response(statuscode.ERROR, "Can only accept recived non-blocked friends");
                    }
                }
            }
            return new Response(statuscode.NOT_THESE_DROIDS, "no friend with that ID, try refreshing the data with UpdateFriends()");
        }

        /// <summary>
        /// Blocks the selected friend, works if pending or already a friend. (you can't block others)
        /// </summary>
        /// <param name="friendID">The user to block</param>
        /// <returns>The sadness in you heart</returns>
        public async Task<Response> RemoveFriend(int friendID)
        {
            foreach (User user in friendarray)
            {
                if (user.ID == friendID)
                {
                    var values = new Dictionary<string, string>
            {
                { "id", UID.ToString() },
                { "friendid", friendID.ToString() }
            };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("http://ec2-3-82-249-155.compute-1.amazonaws.com:3000/friends/removefriend", content);

                    var responseString = await response.Content.ReadAsStringAsync();
                    //Read the data determine the result
                    dynamic obj = JsonConvert.DeserializeObject(responseString);
                    if (!obj.Error)
                    {
                        friendarray.Remove(user);

                        return new Response(statuscode.OK, "Friend removed");
                    }
                    return new Response(statuscode.ERROR, "Could not open database");

                }
            }
            return new Response(statuscode.NOT_THESE_DROIDS, "no friend with that ID, try refreshing the data with UpdateFriends()");
        }

        /// <summary>
        /// Adds an event to the current users scedual.
        /// </summary>
        /// <param name="thing"> the event to add, it not called 'event' because that's a keyword</param>
        /// <returns>If the event was sucessfully added</returns>
        public async Task<Response> AddEvent(Event thing)
        {
            if (IsLoggedIn())
            {
                var values = new Dictionary<string, string>
            {
                { "id", UID.ToString() },
                { "shared", thing.shared.ToString() },
                { "eventname", thing.eventName },
                { "notes", thing.notes },
                { "location", thing.location },
                { "timestart", thing.startTime },
                { "timeend", thing.endTime },
                { "day", thing.Day.ToString() }
            };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("http://ec2-3-82-249-155.compute-1.amazonaws.com:3000/events/addevent", content);

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic obj = JsonConvert.DeserializeObject(responseString);

                if (!obj.Error)
                {
                    int id = int.Parse(obj.EventID);

                    events.Add(new Event(id, UID, thing.eventName, thing.shared, thing.startTime, thing.endTime, thing.Day));
                    //close connection
                    this.CloseConnection();

                    return new Response(statuscode.OK, "event added sucessfully");
                }
                return new Response(statuscode.ERROR, "Could not open database");
            }
            return new Response(statuscode.ERROR, "User not logged in");
        }

        /// <summary>
        /// Replaces an old event with a new one. (untested)
        /// </summary>
        /// <param name="oldEvent">the event pre-edit</param>
        /// <param name="newEvent">the event post-edit</param>
        /// <returns>If the event was sucessfully edited</returns>
        public async Task<Response> EditEvent(Event oldEvent, Event newEvent)
        {
            if (IsLoggedIn())
            {
                var values = new Dictionary<string, string>
            {
                { "id", UID.ToString() },
                { "eventid", oldEvent.eID.ToString() },
                { "shared", newEvent.shared.ToString() },
                { "eventname", newEvent.eventName },
                { "notes", newEvent.notes },
                { "location", newEvent.location },
                { "timestart", newEvent.startTime },
                { "timeend", newEvent.endTime },
                { "day", newEvent.Day.ToString() }
            };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("http://ec2-3-82-249-155.compute-1.amazonaws.com:3000/events/editevent", content);

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic obj = JsonConvert.DeserializeObject(responseString);
                if (!obj.Error)
                {
                    events.Remove(oldEvent);

                    events.Add(new Event(oldEvent.eID, UID, newEvent.eventName, newEvent.shared, newEvent.startTime, newEvent.endTime, newEvent.Day));

                    return new Response(statuscode.OK, "event edited sucessfully");
                }
                return new Response(statuscode.ERROR, "Could not open database");
            }
            return new Response(statuscode.ERROR, "User not logged in");
        }

        /// <summary>
        /// Removes the selected event from the current users schedual. (untested)
        /// </summary>
        /// <param name="thing">the event to remove, it not called 'event' because that's a keyword</param>
        /// <returns>If the event was 'taken out of the picture', if you know what I mean...</returns>
        public Response RemoveEvent(Event thing)
        {
            if (!myevents.Contains(thing))
                return new Response(statuscode.ERROR, "That event could not be found, try running updateEvents()");

            string query = "DELETE FROM `events` WHERE `EventID` = " + thing.eID + " ";
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
                events.Remove(thing);

                return new Response(statuscode.OK, "Event removed");
            }
            return new Response(statuscode.ERROR, "Could not open database");
        }

        /// <summary>
        /// Gets the current user, and their friends events.
        /// </summary>
        /// <returns>An array of events</returns>
        public Event[] GetEvents()
        {
            if (IsLoggedIn())
            {
                if (events.Count != 0)
                {
                    List<Event> e = new List<Event>();
                    e.AddRange(events);
                    e.AddRange(myevents);
                    return e.ToArray();

                }
                return UpdateEvents();
            }
            return new Event[] { };
        }

        /// <summary>
        /// Updates the local copy of personal and shared events.
        /// </summary>
        /// <returns>all relevant events</returns>
        public Event[] UpdateEvents()
        {
            events.Clear();
            myevents.Clear();
            string inquery;
            if (friendarray.Count != 0)
            {
                inquery = "(" + friendarray[0].ID;
                for (int i = 1; i < friendarray.Count; i++)
                    inquery = inquery + ", " + friendarray[i].ID;
                inquery = inquery + ")";
            }
            else
            {
                return new Event[] { };
            }
            string query = "SELECT * FROM `event` WHERE `ID` IN " + inquery + " AND `shared` = true";

            //Create a list to store the result
            List<Event> list = new List<Event>();

            //get the users events
            Event[] ev = GetMyEvents();
            myevents.AddRange(ev);
            list.AddRange(ev);
            //events.AddRange(ev);

            //then get their friends
            int eID, ID;
            bool s;
            string name, notes, loc, st, et;
            day d;


            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    eID = int.Parse(dataReader["EventID"] + "");
                    ID = int.Parse(dataReader["ID"] + "");
                    s = bool.Parse(dataReader["shared"] + "");
                    name = dataReader["EventName"] + "";
                    notes = dataReader["notes"] + "";
                    loc = dataReader["Location"] + "";
                    st = convert_time(int.Parse(dataReader["TimeStart"] + ""));
                    et = convert_time(int.Parse(dataReader["TimeEnd"] + ""));
                    Enum.TryParse(dataReader["Day"] + "", out d);

                    list.Add(new Event(eID, ID, name, s, st, et, d, loc, notes));
                    events.Add(new Event(eID, ID, name, s, st, et, d, loc, notes));
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list.ToArray();
            }
            else
            {
                return list.ToArray();
            }
        }

        /// <summary>
        /// Gets an array of the current users events. 
        /// </summary>
        /// <returns>an array of the current users events</returns>
        public Event[] GetMyEvents()
        {
            if (myevents.Count != 0)
            {
                return myevents.ToArray();
            }
            string query = "SELECT * FROM `event` WHERE `ID` = " + UID + "";

            //Create a list to store the result
            List<Event> list = new List<Event>();
            int eID, ID;
            bool s;
            string name, notes, loc, st, et;
            day d;


            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    eID = int.Parse(dataReader["EventID"] + "");
                    ID = int.Parse(dataReader["ID"] + "");
                    s = bool.Parse(dataReader["shared"] + "");
                    name = dataReader["EventName"] + "";
                    notes = dataReader["notes"] + "";
                    loc = dataReader["Location"] + "";
                    st = convert_time(int.Parse(dataReader["TimeStart"] + ""));
                    et = convert_time(int.Parse(dataReader["TimeEnd"] + ""));
                    Enum.TryParse(dataReader["Day"] + "", out d);

                    list.Add(new Event(eID, ID, name, s, st, et, d, loc, notes));
                    myevents.Add(new Event(eID, ID, name, s, st, et, d, loc, notes));
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list.ToArray();
            }
            else
            {
                return list.ToArray();
            }
        }

        /// <summary>
        ///  Gets the current users pending, accepted and blocked friends
        /// </summary>
        /// <returns>the users friends and their status</returns>
        public User[] GetFriends()
        {
            if (!IsLoggedIn())
                return friendarray.ToArray();

            if (friendarray.Count != 0)
            {
                return friendarray.ToArray();
            }
            else
            {
                return UpdateFriends();
            }

        }

        /// <summary>
        /// updates the local copy of uesr friend objects, includes of all statuses
        /// </summary>
        /// <returns>an array of the users friends</returns>
        public User[] UpdateFriends()
        {
            string query = " SELECT `Name`, `ID`, `status` AS `!status` FROM (SELECT `Name`, `ID` FROM `users` WHERE `ID` IN (SELECT `ID1` FROM `friends` WHERE `ID2` = " + UID + ")) AS T INNER JOIN `friends` ON `ID` = `ID1` group by `Name`";
            //Create a list to store the result
            List<User> list = new List<User>();
            friendarray.Clear();
            int ID;
            string name;
            friends status;

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    foreach (var item in dataReader)
                    {
                        Console.WriteLine(item.ToString());
                    }
                    ID = int.Parse(dataReader["ID"] + "");
                    name = dataReader["Name"] + "";
                    if ((dataReader["!status"] + "") == "PENDING_TO")
                    {
                        status = friends.PENDING_FROM;
                    }
                    if ((dataReader["!status"] + "") == "BLOCKED_TO")
                    {
                        status = friends.BLOCKED_FROM;
                    }
                    else
                    {
                        Enum.TryParse(dataReader["!status"] + "", out status);
                        Console.WriteLine(dataReader["!status"] + "");
                    }
                    list.Add(new User(ID, name, status));
                    friendarray.Add(new User(ID, name, status));
                }

                //close Data Reader
                dataReader.Close();

                query = "SELECT `Name`, `ID`, `status`  FROM (SELECT `Name`, `ID` FROM `users` WHERE `ID` IN (SELECT `ID2` FROM `friends` WHERE `ID1` = " + UID + ")) AS T INNER JOIN `friends` ON `ID` = `ID2` group by `Name` ";
                //Create Command
                cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    ID = int.Parse(dataReader["ID"] + "");
                    name = dataReader["Name"] + "";
                    Enum.TryParse(dataReader["status"] + "", out status);
                    list.Add(new User(ID, name, status));
                    friendarray.Add(new User(ID, name, status));
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list.ToArray();
            }
            else
            {
                return list.ToArray();
            }
        }

        /// <summary>
        /// Get the user object associated with a given username.
        /// </summary>
        /// <param name="username">the name to get</param>
        /// <returns>the first user returned</returns>
        private User GetUser(string username)
        {
            string query = "select * FROM `users` WHERE `Name` = '" + username + "' LIMIT 1";
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data determine the result
                if (dataReader.Read())
                {
                    int id = int.Parse(dataReader["ID"] + "");
                    //close Data Reader
                    dataReader.Close();

                    //close connection
                    this.CloseConnection();

                    return new User(id, username, friends.PENDING_TO);
                }
                else
                {
                    //close Data Reader
                    dataReader.Close();

                    //close connection
                    this.CloseConnection();
                    return new User(0, "", friends.NO);
                }
            }
            return new User(0, "", friends.NO);
        }

        /// <summary>
        /// Preforms whichcraft and wizardry
        /// </summary>
        /// <param name="time">an int wiht the time in 24hour format</param>
        /// <returns>time in the format hh:mm</returns>
        private string convert_time(int time)
        {
            string t = time.ToString();

            if (t.Length == 3)
            {
                return "0" + t[0] + ":" + t[1] + t[2];
            }
            else
            {
                return t[0] + t[1] + ":" + t[2] + t[3];
            }
        }
    }


    public class Backendtest
    {
        private int idcounter = 0;
        private int UID;
        public string UserName { get; private set; }
        //private string server;
        //private string database;
        //private string uid;
        //private string password;
        private List<string> takenNames;
        private List<User> friendarray;
        private List<Event> events;
        private List<Event> myevents;
        public Backendtest()
        {
            takenNames = new List<string>();
            takenNames.Add("");
            takenNames.Add("Bob");
            takenNames.Add("Jane");
            takenNames.Add("Harry");
            friendarray = new List<User>();
            events = new List<Event>();
            myevents = new List<Event>();
            UID = 0;
        }


        /// <summary>
        /// Is a user currently logged in, does not refer to database connectivity.
        /// </summary>
        /// <returns>the statments truth</returns>
        public bool IsLoggedIn()
        {
            return (UID == 0 ? false : true);
        }

        /// <summary>
        /// Queries the database to see if that username is taken yet.
        /// </summary>
        /// <param name="username">the username to search for</param>
        /// <returns>if the username is currently not in use</returns>
        public bool IsUsernameAvalible(string username)
        {
            if (takenNames.Contains(username))
                return false;
            return true;
        }

        #region sha256 string generator, from https://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
        private static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// Logs the user in, actually It queries the database to confirm details, then sets the users ID as the active user.
        /// </summary>
        /// <param name="username"> the users username</param>
        /// <param name="password"> the users password, this is not ever stored anyware</param>
        /// <returns> A response</returns>
        public Response LogIn(string username, string password)
        {
            bool Avalible;
            string psh = GetHashString(password);
            Console.WriteLine(psh);
            if (takenNames.Contains(username))
            {
                Avalible = true;
            }
            else
                Avalible = false;
            if (Avalible)
            {
                UID = takenNames.IndexOf(username);
                return new Response(statuscode.OK, UID.ToString());
            }
            return new Response(statuscode.NOT_THESE_DROIDS, "Username or password was incorrect");
            
        }

        /// <summary>
        /// Logs the user out, actually un-asgines the current user value.
        /// </summary>
        /// <returns>A response, not really useful</returns>
        public Response LogOut()
        {
            UID = 0;
            UserName = "";
            friendarray = new List<User>();
            events = new List<Event>();
            myevents = new List<Event>();
            return new Response(statuscode.OK, "Nothing to see here!");
        }

        /// <summary>
        /// Registers the user in the database, you should probobly check if the username is avalible first.
        /// note: this does not log in the new user.
        /// </summary>
        /// <param name="username">the desired username</param>
        /// <param name="password">the desired password, (not stored)</param>
        /// <returns>OK if its all good, or error if the user already exists</returns>
        public Response SignUp(string username, string password)
        {
            if (IsUsernameAvalible(username))
            {

                return new Response(statuscode.OK, "User registered sucessfully");
            }
            else
            {
                return new Response(statuscode.NOT_THESE_DROIDS, "Username is not avalible");
            }

        }

        /// <summary>
        /// Changes the current users username, to an avalible username.
        /// </summary>
        /// <param name="newUsername">the new username to give</param>
        /// <returns>a response</returns>
        public Response ChangeUsername(string newUsername)
        {
            if (IsLoggedIn())
            {
                if (IsUsernameAvalible(newUsername))
                {
                    takenNames.Remove(this.UserName);
                    takenNames.Add(newUsername);
                    this.UserName = newUsername;
                    return new Response(statuscode.OK, "Username Updated");
                }
                return new Response(statuscode.ERROR, "That username is not avalible");
            }
            return new Response(statuscode.ERROR, "user not logged in");
        }

        /// <summary>
        /// Changes the current users password to a new one, requires the user to verify their password to do so.
        /// </summary>
        /// <param name="newPassword">the new password</param>
        /// <param name="oldPassword">the old password</param>
        /// <returns>a response displaying the result</returns>
        public Response ChangePassword(string newPassword, string oldPassword)
        {
            if (IsLoggedIn())
            {
                if (LogIn(UserName, oldPassword).status == statuscode.OK)
                {
                    string newHash = GetHashString(newPassword);
                    return new Response(statuscode.OK, "Paswsword updated");
                }
                return new Response(statuscode.NOT_THESE_DROIDS, "Invalid current password");
            }
            return new Response(statuscode.ERROR, "user not logged in");
        }

        /// <summary>
        /// Sends a friend request to the selected user.
        /// </summary>
        /// <param name="user">the user to send to</param>
        /// <returns>OK if all good, Error if blocked, already a friend, or the user dosen't exist</returns>
        public Response RequestFriend(User user)
        {
            if (IsLoggedIn())
            {
                if (!IsUsernameAvalible(user.username))
                {
                        friendarray.Add(user);
                        return new Response(statuscode.OK, "Friend request sent sucessfully");
                
                }
                else
                {
                    return new Response(statuscode.NOT_THESE_DROIDS, "That user does not exist");
                }
            }
            else
            {
                return new Response(statuscode.ERROR, "User is not logged in!");
            }
        }

        /// <summary>
        /// sends a friend request to the selected user.
        /// </summary>
        /// <param name="username"> the user to send to</param>
        /// <returns>If the user was sucessfully requested</returns>
        public Response RequestFriend(string username)
        {
            if (IsLoggedIn())
            {
                return RequestFriend(GetUser(username));
            }
            return new Response(statuscode.ERROR, "User not logged in");
        }

        /// <summary>
        /// sends a friend request to the selected user.
        /// </summary>
        /// <param name="userID"> the ID of the user to send to</param>
        /// <returns>If the user was sucessfully requested</returns>
        public Response RequestFriend(int userID)
        {
            if (IsLoggedIn())
            {
                return RequestFriend(new User(userID, "", friends.NO));
            }
            return new Response(statuscode.ERROR, "User not logged in");
        }

        /// <summary>
        /// The back backend function which updates friend requests, not sure if it works properly...
        /// </summary>
        /// <param name="newStatus">the stats to set the friend to</param>
        /// <param name="friendID">the friend in question</param>
        /// <param name="to">which direction was the request sent?</param>
        /// <returns>a response</returns>
        private Response UpdateFriendRequestStatus(friends newStatus, int friendID, bool to)
        {
            if (IsLoggedIn())
            {
                int index = 0;
                foreach (User fri in friendarray)
                {
                    if (fri.ID == friendID)
                    {
                        if (newStatus == friends.NO)
                            return RemoveFriend(friendID);

                        
                            friendarray[index] = new User(friendarray[index].ID, friendarray[index].username, newStatus);

                            return new Response(statuscode.OK, "Friend request Updated sucessfully");
                    }
                    index++;
                }
                return new Response(statuscode.NOT_THESE_DROIDS, "You don't have a friend with that ID");
            }
            return new Response(statuscode.ERROR, "User not logged in");
        }

        /// <summary>
        /// Accepts a pending friend request.
        /// </summary>
        /// <param name="friendID">the ID of the request to accept</param>
        /// <returns> if the acceptance was sucessful</returns>
        public Response AcceptFriend(int friendID)
        {
            foreach (User user in friendarray)
            {
                if (user.ID == friendID)
                {
                    if (user.friend == friends.PENDING_TO)
                    {
                        return UpdateFriendRequestStatus(friends.YES, friendID, true);
                    }
                    else if (user.friend == friends.PENDING_FROM)
                    {
                        return new Response(statuscode.ERROR, "Can only accept recived non-blocked friends");
                    }
                }
            }
            return new Response(statuscode.NOT_THESE_DROIDS, "no friend with that ID, try refreshing the data with UpdateFriends()");
        }

        /// <summary>
        /// Blocks the selected friend, works if pending or already a friend. (you can't block others)
        /// </summary>
        /// <param name="friendID">The user to block</param>
        /// <returns>The sadness in you heart</returns>
        public Response RemoveFriend(int friendID)
        {
            foreach (User user in friendarray)
            {
                if (user.ID == friendID)
                { 
                        friendarray.Remove(user);
                        return new Response(statuscode.OK, "Friend removed");
                }
            }
            return new Response(statuscode.NOT_THESE_DROIDS, "no friend with that ID, try refreshing the data with UpdateFriends()");
        }

        /// <summary>
        /// Adds an event to the current users scedual.
        /// </summary>
        /// <param name="thing"> the event to add, it not called 'event' because that's a keyword</param>
        /// <returns>If the event was sucessfully added</returns>
        public Response AddEvent(Event thing)
        {
            if (IsLoggedIn())
            {

                int id = idcounter;
                idcounter++;
                    events.Add(new Event(id, UID, thing.eventName, thing.shared, thing.startTime, thing.endTime, thing.Day));


                    return new Response(statuscode.OK, "event added sucessfully");
                
            }
            return new Response(statuscode.ERROR, "User not logged in");
        }

        /// <summary>
        /// Replaces an old event with a new one. (untested)
        /// </summary>
        /// <param name="oldEvent">the event pre-edit</param>
        /// <param name="newEvent">the event post-edit</param>
        /// <returns>If the event was sucessfully edited</returns>
        public Response EditEvent(Event oldEvent, Event newEvent)
        {
            if (IsLoggedIn())
            {
               
                    events.Remove(oldEvent);

                    events.Add(new Event(oldEvent.eID, UID, newEvent.eventName, newEvent.shared, newEvent.startTime, newEvent.endTime, newEvent.Day));
          
                    return new Response(statuscode.OK, "event edited sucessfully");
           
            }
            return new Response(statuscode.ERROR, "User not logged in");
        }

        /// <summary>
        /// Removes the selected event from the current users schedual. (untested)
        /// </summary>
        /// <param name="thing">the event to remove, it not called 'event' because that's a keyword</param>
        /// <returns>If the event was 'taken out of the picture', if you know what I mean...</returns>
        public Response RemoveEvent(Event thing)
        {
            if (!events.Contains(thing))
                return new Response(statuscode.ERROR, "That event could not be found, try running updateEvents()");

           
                events.Remove(thing);

                return new Response(statuscode.OK, "Event removed");
        }

        /// <summary>
        /// Gets the current user, and their friends events.
        /// </summary>
        /// <returns>An array of events</returns>
        public Event[] GetEvents()
        {
            if (IsLoggedIn())
            {
                if (events.Count != 0)
                {
                    List<Event> e = new List<Event>();
                    e.AddRange(events);
                    e.AddRange(myevents);
                    return e.ToArray();

                }
                return UpdateEvents();
            }
            return new Event[] { };
        }

        /// <summary>
        /// Updates the local copy of personal and shared events.
        /// </summary>
        /// <returns>all relevant events</returns>
        public Event[] UpdateEvents()
        {
            events.Clear();
            myevents.Clear();
            if (friendarray.Count == 0)
            {
                return new Event[] { };
            }


            //Create a list to store the result
            List<Event> list = new List<Event>();



            list.Add(new Event(1, 1, "IAB330", true, "09:45", "11:00", day.Mon, "", "Mobile app development"));
            events.Add(new Event(1, 1, "IAB330", true, "09:45", "11:00", day.Mon, "", "Mobile app development"));

            list.Add(new Event(2, 1, "IFB101", true, "09:00", "10:00", day.Tue, "", "People and the context of technology"));
            events.Add(new Event(2, 1, "IFB101", true, "09:00", "10:00", day.Tue, "", "People and the context of technology"));

            list.Add(new Event(3, 1, "IFB101 Lecture", true, "13:00", "15:00", day.Fri, "Z-411", "People and the context of technology"));
            events.Add(new Event(3, 1, "IFB101 Lecture", true, "13:00", "15:00", day.Fri, "Z-411", "People and the context of technology"));


            list.Add(new Event(4, 2, "IFB101 Lecture", true, "13:00", "15:00", day.Fri, "Z-411", "That bad class that's getting cancled"));
            events.Add(new Event(4, 2, "IFB101 Lecture", true, "13:00", "15:00", day.Fri, "Z-411", "That bad class that's getting cancled"));

            list.Add(new Event(5, 2, "MXB111", true, "18:00", "20:00", day.Sat, "B-110", "Literly the worst"));
            events.Add(new Event(5, 2, "MXB111", true, "18:00", "20:00", day.Sat, "B-110", "Literly the worst"));

            list.Add(new Event(6, 2, "IGB110 Lecture", false, "08:30", "10:30", day.Mon, "Z-401", "No comment"));
            events.Add(new Event(6, 2, "IGB110 Lecture", false, "08:30", "10:30", day.Mon, "Z-401", "No comment"));


            list.Add(new Event(7, 3, "IAB330", true, "18:00", "20:00", day.Mon, "S-509", "Mobile"));
            events.Add(new Event(7, 3, "IAB330", true, "18:00", "20:00", day.Mon, "S-509", "Mobile"));

            list.Add(new Event(8, 3, "IAB330 Lecture", true, "15:00", "17:00", day.Tue, "Z-509", "Mobile"));
            events.Add(new Event(8, 3, "IAB330 Lecture", true, "15:00", "17:00", day.Tue, "Z-509", "Mobile"));

            list.Add(new Event(9, 3, "Do not Disturb", true, "09:00", "17:00", day.Fri, "", "None of your business"));
            events.Add(new Event(9, 3, "Do not Disturb", true, "09:00", "17:00", day.Fri, "", "None of your business"));



            Event[] ev = GetMyEvents();
            myevents.AddRange(ev);
            list.AddRange(ev);
            return list.ToArray();
            
        }

        /// <summary>
        /// Gets an array of the current users events. 
        /// </summary>
        /// <returns>an array of the current users events</returns>
        public Event[] GetMyEvents()
        {
            if (myevents.Count != 0)
            {
                return myevents.ToArray();
            }

            List<Event> list = new List<Event>();

            foreach(Event e in events)
            {
                if(e.ID == UID)
                {
                    list.Add(e);
                }
            }

            return list.ToArray();
            
        }

        /// <summary>
        ///  Gets the current users pending, accepted and blocked friends
        /// </summary>
        /// <returns>the users friends and their status</returns>
        public User[] GetFriends()
        {
            if (!IsLoggedIn())
                return friendarray.ToArray();

            if (friendarray.Count != 0)
            {
                return friendarray.ToArray();
            }
            else
            {
                return UpdateFriends();
            }

        }

        /// <summary>
        /// updates the local copy of uesr friend objects, includes of all statuses
        /// </summary>
        /// <returns>an array of the users friends</returns>
        public User[] UpdateFriends()
        {
            //Create a list to store the result
            List<User> list = new List<User>();
            friendarray.Clear();
   
            list.Add(new User(1, "Bob", friends.YES));
            list.Add(new User(2, "Jane", friends.YES));
            list.Add(new User(3, "Harry", friends.YES));
            friendarray.Add(new User(1, "Bob", friends.YES));
            friendarray.Add(new User(2, "Jane", friends.YES));
            friendarray.Add(new User(3, "Harry", friends.YES));

            //return list to be displayed
            return list.ToArray();
  
        }

        /// <summary>
        /// Get the user object associated with a given username.
        /// </summary>
        /// <param name="username">the name to get</param>
        /// <returns>the first user returned</returns>
        private User GetUser(string username)
        {
            if (takenNames.Contains(username))
            {
                foreach(var friend in friendarray)
                {
                    if(friend.username == username)
                    {
                        return friend;
                    }
                }
            }
            return new User(0, "", friends.NO);
        }

        /// <summary>
        /// Preforms whichcraft and wizardry
        /// </summary>
        /// <param name="time">an int wiht the time in 24hour format</param>
        /// <returns>time in the format hh:mm</returns>
        private string convert_time(int time)
        {
            string t = time.ToString();

            if (t.Length == 3)
            {
                return "0" + t[0] + ":" + t[1] + t[2];
            }
            else
            {
                return t[0] + t[1] + ":" + t[2] + t[3];
            }
        }
    }
}


