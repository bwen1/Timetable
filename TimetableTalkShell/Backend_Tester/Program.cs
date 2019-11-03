using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using databaseConnector;

namespace Backend_Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Console based backend tester V0.01");
            Console.WriteLine("Initilising Backend");
            Backend bk = new Backend();
            Console.WriteLine("done");
            string com = "";
            while (true)
            {
                Console.Write("do:>");
                com = Console.ReadLine();
                if (com == "check login")
                {
                    check_login(bk);
                }
                else if (com == "check username")
                {
                    check_username_avalibility(bk);
                }
                else if (com == "login")
                {
                    account_login(bk);
                }
                else if (com == "logout")
                {
                    bk.LogOut();
                    Console.WriteLine("Logged out!");
                }
                else if (com == "sign up")
                {
                    account_signup(bk);
                }
                else if (com == "request friend")
                {
                    request_friend(bk);
                }
                else if (com == "accept friend")
                {
                    accept_friend(bk);
                }
                else if (com == "remove friend")
                {
                    remove_friend(bk);
                }
                else if (com == "get friends")
                {
                    get_friends(bk);
                }
                else if (com == "get events")
                {
                    get_events(bk);
                }
                else if (com == "update events")
                {
                    update_events(bk);
                }
                else if (com == "get my events")
                {
                    get_my_events(bk);
                }
                else if (com == "quit")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("invalid input, avalible: check login, check username, login, sign up\nget friends, request friend, accept friend, remove friend,\nget events, update events, get my events, quit");
                }
            }

            // Console.Write("Now register a new user, Username: ");


            Console.WriteLine("End of Current tests, press any key to exit...");
            Console.ReadKey();

        }

        static public void check_login(Backend bk)
        {
            Console.WriteLine("checking to see if the user is logged in...");
            Console.WriteLine("It appears that the user is " + (bk.IsLoggedIn() ? "Currently logged in" : "Not loged in"));
        }

        static public async void check_username_avalibility(Backend bk)
        {
            Console.Write("Enter a username to check avalibility: ");
            string nametest = Console.ReadLine();
            Console.WriteLine("The username appears to be " + (await bk.IsUsernameAvalible(nametest) ? "Avalible" : "in use"));
        }
        static public async void account_login(Backend bk)
        {
            Console.Write("Now login to an account, Username: ");
            string loginntest = Console.ReadLine();
            Console.Write("Password: ");
            string loginptest = Console.ReadLine(); 
            Response loginresp = await bk.LogIn(loginntest, loginptest);
            Console.WriteLine("The response from the login was\n\tCode: " + loginresp.status.ToString() + "\n\tMessage: " + loginresp.message);
        }

        static public async void account_signup(Backend bk)
        {
            Console.Write("Now create an account, Username: ");
            string usr = Console.ReadLine();
            Console.Write("\tPassword: ");
            string pas = Console.ReadLine();
            Console.WriteLine("Createing account...");
            Response resp = await bk.SignUp(usr, pas);
            Console.WriteLine("The response from the signup was\n\tCode: " + resp.status.ToString() + "\n\tMessage: " + resp.message);
        }

        static public void get_friends(Backend bk)
        {
            Console.WriteLine("Getting Users...");
            User[] friends = bk.GetFriends();
            foreach(User user in friends)
            {
                Console.WriteLine("\nUser: " + user.username);
                Console.WriteLine("\tID: " + user.ID);
                Console.WriteLine("\tStatus: " + user.friend);
            }
        }

        static public void request_friend(Backend bk)
        {
            Console.Write("Now request a friend, Username: ");
            string usr = Console.ReadLine();
            Console.WriteLine("requesting friend...");
            Response res = bk.RequestFriend(usr);
            Console.WriteLine("The response from the friend request was\n\tCode: " + res.status.ToString() + "\n\tMessage: " + res.message);
        }

        public static void accept_friend(Backend bk)
        {
            Console.Write("Now accept a friend, FriendID: ");
            int id = int.Parse(Console.ReadLine());
            Console.WriteLine("Accepting friend...");
            Response res = bk.AcceptFriend(id);
            Console.WriteLine("The response from the friend acceptance was\n\tCode: " + res.status.ToString() + "\n\tMessage: " + res.message);
        }

        public static void remove_friend(Backend bk)
        {
            Console.Write("Now remove a friend, FriendID: ");
            int id = int.Parse(Console.ReadLine());
            Console.WriteLine("removing friend...");
            Response res = bk.RemoveFriend(id);
            Console.WriteLine("The response from the friend removal was\n\tCode: " + res.status.ToString() + "\n\tMessage: " + res.message);
        }

        public static void get_events(Backend bk)
        {
            Console.WriteLine("Getting all events...");
            Event[] events = bk.GetEvents();
            foreach(Event e in events)
            {
                Console.WriteLine("\nEvent, Name: " + e.eventName + "\n\tOwner: " + e.ID + "\n\tShared: " + (e.shared ? "yes" : "no") +"\n\tDay: "+e.Day.ToString()+"\n\tLocation: "+e.location+ "\n\tNotes: " + e.notes);
            }
        }
        public static void get_my_events(Backend bk)
        {
            Console.WriteLine("Getting user events...");
            Event[] events = bk.GetMyEvents();
            foreach (Event e in events)
            {
                Console.WriteLine("\nEvent, Name: " + e.eventName + "\n\tOwner: " + e.ID + "\n\tShared: " + (e.shared ? "yes" : "no") + "\n\tDay: " + e.Day.ToString() + "\n\tLocation: " + e.location + "\n\tNotes: " + e.notes);
            }
        }
        public static void update_events (Backend bk)
        {
            Console.WriteLine("Updating local copy of events...");
            if(bk.UpdateEvents().Length != 0)
            {
                Console.WriteLine("done!");
            }
            else
            {
                Console.WriteLine("error");
                Event[] events = bk.UpdateEvents();
                foreach (Event e in events)
                {
                    Console.WriteLine("\nEvent, Name: " + e.eventName + "\n\tOwner: " + e.ID + "\n\tShared: " + (e.shared ? "yes" : "no") + "\n\tDay: " + e.Day.ToString() + "\n\tLocation: " + e.location + "\n\tNotes: " + e.notes);
                }
            }

        }

        public static void create_event()
        {
            Console.WriteLine("Create event, event_name: ");
            string en = Console.ReadLine();
            Console.Write("\n\t");
        }
    }
}
