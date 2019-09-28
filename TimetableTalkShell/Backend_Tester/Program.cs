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
                else if (com == "quit")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("invalid input, avalible: check login, check username, login, sign up, quit");
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

        static public void check_username_avalibility(Backend bk)
        {
            Console.Write("Enter a username to check avalibility: ");
            string nametest = Console.ReadLine();
            Console.WriteLine("The username appears to be " + (bk.IsUsernameAvalible(nametest) ? "Avalible" : "in use"));
        }
        static public void account_login(Backend bk)
        {
            Console.Write("Now login to an account, Username: ");
            string loginntest = Console.ReadLine();
            Console.Write("Password: ");
            string loginptest = Console.ReadLine();
            Response loginresp = bk.LogIn(loginntest, loginptest);
            Console.WriteLine("The response from the login was\n\tCode: " + loginresp.status.ToString() + "\n\tMessage: " + loginresp.message);
        }

        static public void account_signup(Backend bk)
        {
            Console.Write("Now create an account, Username: ");
            string usr = Console.ReadLine();
            Console.Write("\tPassword: ");
            string pas = Console.ReadLine();
            Console.WriteLine("Createing account...");
            Response resp = bk.SignUp(usr, pas);
            Console.WriteLine("The response from the signup was\n\tCode: " + resp.status.ToString() + "\n\tMessage: " + resp.message);
        }
    }
}
