using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

namespace databaseConnector
{
    public enum statuscode { OK, ERROR, NOT_THESE_DROIDS, INVALID_DATA }
    public enum day { MONDAY, TUESDAY, WEDNSDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY }
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
            Initialize();
            takenNames = new List<string>();
            takenNames.Add("");
            friendarray = new List<User>();
            events = new List<Event>();
            myevents = new List<Event>();
            UID = 0;
        }

        #region Database setup, and connection Statments adapted from some online tutorial.
        private void Initialize()
        {
            server = "timetable.ctymoh38xb5w.us-east-1.rds.amazonaws.com";
            database = "timetable_app";
            uid = "admin";
            password = "CqEcjW7Pe8eDruMhzsiU";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        #endregion

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
            bool Avalible;
            if (takenNames.Contains(username))
                return false;

            string query = "SELECT EXISTS(select * FROM `users` WHERE `Name` = '" + username + "') AS `Exists`";
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                MySqlDataReader dataReader = cmd.ExecuteReader();
                dataReader.Read();

                //Read the data determine the result
                if (int.Parse(dataReader["Exists"] + "") == 1)
                {
                    Avalible = false;
                    takenNames.Add(username);
                }
                else
                {
                    Avalible = true;
                }

                //close Data Reader
                dataReader.Close();

                //close connection
                this.CloseConnection();

                return Avalible;
            }
            return false;
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
            string query = "SELECT EXISTS(select * FROM `users` WHERE `Name` = '" + username + "' AND `passwordHash` = '" + psh + "') AS `Exists`";
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                MySqlDataReader dataReader = cmd.ExecuteReader();
                dataReader.Read();

                if (int.Parse(dataReader["Exists"] + "") == 1)
                {
                    dataReader.Close();
                    Avalible = true;
                    string newquery = "select `ID` FROM `users` WHERE `Name` = '" + username + "' AND `passwordHash` = '" + psh + "'";
                    cmd = new MySqlCommand(newquery, connection);
                    dataReader = cmd.ExecuteReader();
                    dataReader.Read();
                    UID = int.Parse(dataReader["ID"] + "");
                }
                else
                {
                    Avalible = false;
                }

                //close Data Reader
                dataReader.Close();

                //close connection
                this.CloseConnection();
                if (Avalible)
                {
                    return new Response(statuscode.OK, UID.ToString());
                }
                return new Response(statuscode.NOT_THESE_DROIDS, "Username or password was incorrect");
            }

            return new Response(statuscode.ERROR, "Could not open sql database");
        }

        /// <summary>
        /// Logs the user out, actually un-asgines the current user value.
        /// </summary>
        /// <returns>A response, not really useful</returns>
        public Response LogOut()
        {
            UID = 0;
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
                string query = "insert into `users` (`Name`, `passwordHash`) values ( '" + username + "', '" + GetHashString(password) + "')";
                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();

                    return new Response(statuscode.OK, "User registered sucessfully");
                }
                return new Response(statuscode.ERROR, "Could not open database");
            }
            else
            {
                return new Response(statuscode.NOT_THESE_DROIDS, "Username is not avalible");
            }

        }

        public Response ChangeUsername(string newUsername)
        {
            return new Response(statuscode.ERROR, "dummy message");
        }

        public Response ChangePassword(string newPassword)
        {
            return new Response(statuscode.ERROR, "dummy message");
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
                    string query = "insert into `friends` values ( " + UID + ", " + user.ID + ", 'PENDING_TO')";
                    if (this.OpenConnection() == true)
                    {
                        //create command and assign the query and connection from the constructor
                        MySqlCommand cmd = new MySqlCommand(query, connection);

                        //Execute command
                        cmd.ExecuteNonQuery();

                        //close connection
                        this.CloseConnection();
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

                        string query = "update `friends` SET `status`='" + newStatus.ToString() + "' WHERE " + (to ? "`ID2`" : "`ID1`") + " = " + friendID + " ";
                        if (this.OpenConnection() == true)
                        {
                            //create command and assign the query and connection from the constructor
                            MySqlCommand cmd = new MySqlCommand(query, connection);

                            //Execute command
                            cmd.ExecuteNonQuery();

                            //close connection
                            this.CloseConnection();
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
        public Response AcceptFriend(int friendID)
        {
            foreach(User user in friendarray)
            {
                if(user.ID == friendID)
                {
                    if(user.friend == friends.PENDING_TO)
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
                    
                        string query = "DELETE FROM `friends` WHERE (`ID1` = "+friendID+" AND `ID2` = "+UID+ ") OR (`ID2` = " + friendID + " AND `ID1` = " + UID + ")";
                        if (this.OpenConnection() == true)
                        {
                            //create command and assign the query and connection from the constructor
                            MySqlCommand cmd = new MySqlCommand(query, connection);

                            //Execute command
                            cmd.ExecuteNonQuery();

                            //close connection
                            this.CloseConnection();
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
        public Response AddEvent(Event thing)
        {
            if (IsLoggedIn())
            {
                string query = "insert into `events` (`ID`, `shared`,`EventName`, `notes`, `Location`, `TimeStart`, `TimeEnd`, `Day`) values ( " + UID + ", "+thing.shared+", '" + thing.eventName + "', '" + thing.notes + "', '" + thing.location + "', " + thing.startTime + ", " + thing.endTime + ", '"+thing.Day.ToString()+"' )";
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    query = "select `EventID` from `events` where `ID` = " + UID + " AND eventName = '" + thing.eventName + "' AND TimeStart = " + thing.startTime + " LIMIT 1";
                    cmd = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    dataReader.Read();
                    int id = int.Parse(dataReader["EventID"] + "");

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
        public Response EditEvent(Event oldEvent, Event newEvent)
        {
            if (IsLoggedIn())
            {
                string query = "UPDATE `events` SET  `ID` =" + UID + ", `shared`= " + newEvent.shared + ", `EventName`= '" + newEvent.eventName + "', `notes`= '" + newEvent.notes + "', `Location`= '" + newEvent.location + "', `TimeStart`= " + newEvent.startTime + ", `TimeEnd`= " + newEvent.endTime + ", `Day`= '" + newEvent.Day.ToString() + "' WHERE `EventID` = "+oldEvent.eID+"";
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();
                    events.Remove(oldEvent);

                    events.Add(new Event(oldEvent.eID, UID, newEvent.eventName, newEvent.shared, newEvent.startTime, newEvent.endTime, newEvent.Day));
                    //close connection
                    this.CloseConnection();

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
            if (!events.Contains(thing))
                return new Response(statuscode.ERROR, "That event could not be found, try running updateEvents()");

            string query = "DELETE FROM `events` WHERE `EventID` = "+thing.eID+" ";
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
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private string convert_time(int time)
        {
            string t = time.ToString();

            if(t.Length == 3)
            {
                return "0" + t[0] + ":" + t[1] + t[2];
            }
            else
            {
                return t[0] + t[1] + ":" + t[2] + t[3];
            }
        }

        /// <summary>
        /// Gets an array of the current users events. allways from database
        /// </summary>
        /// <returns>an array of the current users events</returns>
        public Event[] GetMyEvents()
        {
            if(myevents.Count != 0)
            {
                return myevents.ToArray();
            }
            string query = "SELECT * FROM `event` WHERE `ID` = " + UID + "";

            //Create a list to store the result
            List<Event> list = new List<Event>();
            int eID, ID;
            bool s;
            string name, notes, loc,st, et;
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
                    Enum.TryParse(dataReader["Day"]+"", out d);

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

            if(friendarray.Count != 0)
            {
                return friendarray.ToArray();
            }
            else
            {
                return UpdateFriends();
            }
            
        }

        /// <summary>
        /// BROKEN, recivers of friend requests see whrong status, eveything else works
        /// </summary>
        /// <returns>an array of the users friends</returns>
        public User[] UpdateFriends()
        {
            string query = " SELECT `Name`, `ID`, `status` AS `!status` FROM (SELECT `Name`, `ID` FROM `users` WHERE `ID` IN (SELECT `ID1` FROM `friends` WHERE `ID2` = "+UID+")) AS T INNER JOIN `friends` ON `ID` = `ID1` group by `Name`";
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
                        Console.WriteLine(dataReader["!status"]+"");
                    }
                    list.Add(new User(ID, name, status));
                    friendarray.Add(new User(ID, name, status));
                }

                //close Data Reader
                dataReader.Close();

                query = "SELECT `Name`, `ID`, `status`  FROM (SELECT `Name`, `ID` FROM `users` WHERE `ID` IN (SELECT `ID2` FROM `friends` WHERE `ID1` = "+UID+")) AS T INNER JOIN `friends` ON `ID` = `ID2` group by `Name` ";
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
        /// (unused)Gets the public events of the specified user, (they have to be a friend for this to work) (UN implemented)
        /// </summary>
        /// <param name="user">the user to get events for</param>
        /// <returns>that users events</returns>
        private Event[] GetUserEvents(User user)
        {
            return new Event[] { };
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

    }

    public class Database
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public Database()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "timetable.ctymoh38xb5w.us-east-1.rds.amazonaws.com";
            database = "timetable_app";
            uid = "app";
            password = "appauth";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Insert statement
        public void Insert(string table, string[] values)
        {
            string query = "INSERT INTO " + table + " VALUES('John Smith', '33')";

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        //Update statement
        public void Update()
        {
            string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = connection;

                //Execute query
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        //Delete statement
        public void Delete()
        {
            string query = "DELETE FROM tableinfo WHERE name='John Smith'";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        //Select statement
        public List<string>[] Select()
        {
            string query = "SELECT * FROM tableinfo";

            //Create a list to store the result
            List<string>[] list = new List<string>[3];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();

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
                    list[0].Add(dataReader["id"] + "");
                    list[1].Add(dataReader["name"] + "");
                    list[2].Add(dataReader["age"] + "");
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }
        }

        //Count statement
        public int Count()
        {
            string query = "SELECT Count(*) FROM tableinfo";
            int Count = -1;

            //Open Connection
            if (this.OpenConnection() == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                this.CloseConnection();

                return Count;
            }
            else
            {
                return Count;
            }
        }

        //Backup
        public void Backup()
        {
        }

        //Restore
        public void Restore()
        {
        }
    }
}
