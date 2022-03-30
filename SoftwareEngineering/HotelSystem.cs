/*Authors: Jonathon Ford, 
 * 
 * This program is a console app for a hotel database. Once ran you can run 15 different opperations which are as follows
 * 1.) Make Reservation (adds customer information and dates they want to book, as well as pricing)
 * 2.) Edit a Reservation (edits a reservation in the system)
 * 3.) Cancel a Reservation (cancles a reservation and charges a card if need be)
 * 4.) Check reservation info (allows an employee/manager to look up reservations and displays the info)
 * 5.) Confirm Reservation (marks a reservation to not be canceled if not checked in)
 * 6.) Check avalability (allows an employee/manager to check hotel avalability for a certain date)
 * 7.) Check in Guest (marks a reservation as checked in)
 * 8.) Check out Guest (marks that a guest has checked out and generates a bill)
 * 9.) Generate daily emails (generates any reminder emails that need to be sent out to potental guests)
 * 10.) Generate daily arrivals report (creates a list of all people that will be checking in that day)
 * 11.) Generate daily occupancy report (creates a list of everyone checking out, staying, and arriving that day)
 * 12.) Generate 30 day occupancy report ( Managers only, generate what reservations are to be had for the next 30 days)
 * 13.) Generate 30 day income report (Managers only, generate a report for how much money will be made in the next 30 days if no one cancles)
 * 14.) Generate 30 day incentive report (Managers only, generate a report for how much money will be lost due to incentive discount)
 * 15.) Set Base Rate (Set a rate for the room pricing for a certain date)
 * 
 */

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
            string command = "";
            while (!String.Equals(command, "q"))//Until the user logs out, print the options and read what they would like to do
            {
                Console.WriteLine("Input your desired function (1 - 15) or q to log out:");
                Console.WriteLine("1  - Make a reservation");
                Console.WriteLine("2  - Edit a reservaation");
                Console.WriteLine("3  - Cancel a reservation");
                Console.WriteLine("4  - Check reservation details");
                Console.WriteLine("5  - Confirm reservation");
                Console.WriteLine("6  - Check availability");
                Console.WriteLine("7  - Check in guest");
                Console.WriteLine("8  - Check out guest");
                Console.WriteLine("9  - Generate daily emails");
                Console.WriteLine("10 - Generate daily arivals report");
                Console.WriteLine("11 - Generate daily occupancy report");
                /*if(String.Equals(currentUser.role, "Management")
                Console.WriteLine("12 - Generate 30 day occupancy report");
                Console.WriteLine("13 - Generate 30 day expected income report");
                Console.WriteLine("14 - Generate 30 day incentive loss report");
                Console.WriteLine("15 - Set base rate");
                */
                command = Console.ReadLine();
                switch (command)
                {
                    case "1":
                        //MakeReservation();
                        break;
                    case "2":
                        //EditReservation();
                        break;
                    case "3":
                        //CancelReservation();
                        break;
                    case "4":
                        //FindReservation();
                        break;
                    case "5":
                        //ConfirmReservation();
                        break;
                    case "6":
                        //CheckAvailability();
                        break;
                    case "7":
                        //CheckInGuest();
                        break;
                    case "8":
                        //CheckOutGuest();
                        break;
                    case "9":
                        //GenerateDailyEmails();
                        break;
                    case "10":
                        //GenerateDailyArrivalsReport();
                        break;
                    case "11":
                        //GenerateDailyOccupancyReport();
                        break;
                    case "12":
                        //if(String.Equals(currentUser.role, "Management"){
                        //  GenerateExpectedOccupancyReport();
                        //}
                        break;
                    case "13":
                        //if(String.Equals(currentUser.role, "Management"){
                        //  GenerateExpectedOccupancyReport();
                        //}
                        break;
                    case "14":
                        //if(String.Equals(currentUser.role, "Management"){
                        //  GenerateExpectedOccupancyReport();
                        //}
                        break;
                    case "15":
                        //if(String.Equals(currentUser.role, "Management"){
                        //  GenerateExpectedOccupancyReport();
                        //}
                        break;
                    case "q":
                        Console.WriteLine("Are you sure you want to log out? y/n");
                        string input = Console.ReadLine();
                        if (String.Equals(input, "y"))//if they are sure do nothing
                        {
                            break;
                        }
                        else //if they change their mind change command so you do not break out the loop
                        {
                            command = "";
                            break;
                        }
                        
                    default:
                        Console.WriteLine("Invalid input, try again");
                        break;
                }
            }


        }

        /*This function queries the database for employees with given username and password, if it matches one it returns true
         * and sets the global user with the credentials
         */
        static bool LogInUser(string username, string password)
        {
            //TODO, check if the credentials match a user if so currentUser == to the specified user
            return true;
        }
        /*This function checks in a guest by updating the database
         * 
         */
        static void CheckInGuest()
        {

        }
        /*This function checks out a guest at the end of their stay
         * 
         */
        static void CheckOutGuest()
        {

        }
        /*This function produces a bill for the customer and "charges their card"
         * 
         */
        static void ProcessPayment()
        {

        }
        /*This function configures base rate
         * 
         */
        static void ConfigureBaseRate()
        {

        }
    }
}
