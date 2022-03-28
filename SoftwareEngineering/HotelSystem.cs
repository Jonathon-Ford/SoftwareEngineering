using System;
using System.Threading;

namespace SoftwareEngineering
{
    class HotelSystem
    {
        //User currentUser;
        static void Main(string[] args)
        {
            int remainingAttempts;
            string username;
            string password;

            while (true) {//Until a user is logged in
                remainingAttempts = 5;
                Console.WriteLine("Hello please input your username:");
                username = Console.ReadLine();
                Console.WriteLine("Please input your password");
                password = Console.ReadLine();
                while (!LogInUser(username, password) && remainingAttempts > 0)//if the user is not logged in and they have remaining attempts
                {
                    Console.WriteLine("Invalid log in, try again");
                    Console.WriteLine("Please input your username:");
                    username = Console.ReadLine();
                    Console.WriteLine("Please input your password");
                    password = Console.ReadLine();

                    remainingAttempts--;
                }

                if(remainingAttempts < 0)//If they made it here because of hitting the attempt limits, sleep for 5 min
                {
                    Console.WriteLine("Too many attempts, locking for 5 min");
                    Thread.Sleep(300000);
                }
                else//If they made it here because of being a valid user
                {
                     break;
                }
            }



        }

        

        static bool LogInUser(string username, string password)
        {
            //TODO, check if the credentials match a user if so currentUser == to the specified user
            return true;
        }
    }
}
