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



Users currentUser = new Users { Password= "", Username = "", RoleName = ""}; // An empty user to keep the compiler happy
static void Main(Users currentUser)
{
    int remainingAttempts;
    string username;
    string password;

    while (true)
    {//Until a user is logged in
        remainingAttempts = 5;
        Console.WriteLine("Hello please input your username:");
        username = Console.ReadLine();
        Console.WriteLine("Please input your password");
        password = Console.ReadLine();
        while (!LogInUser(username, password, currentUser) && remainingAttempts > 0)//if the user is not logged in and they have remaining attempts
        {
            Console.WriteLine("Invalid log in, try again");
            Console.WriteLine("Please input your username:");
            username = Console.ReadLine();
            Console.WriteLine("Please input your password");
            System.Console.Write("password: ");

            //Hidden read
            password = null;
            while (true)
            {
                var key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                password += key.KeyChar;
            }

            remainingAttempts--;
        }

        if (remainingAttempts < 0)//If they made it here because of hitting the attempt limits, sleep for 5 min
        {
            Console.WriteLine("Too many attempts, locking for 5 min");
            Thread.Sleep(300000);
        }
        else//If they made it here its because of being a valid user
        {
            password = ""; // For security reasons scrub the password
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
        if (String.Equals(currentUser.RoleName, "Management")){
            Console.WriteLine("12 - Generate 30 day occupancy report");
            Console.WriteLine("13 - Generate 30 day expected income report");
            Console.WriteLine("14 - Generate 30 day incentive loss report");
            Console.WriteLine("15 - Set base rate");
            Console.WriteLine("16 - Create new user");
            Console.WriteLine("17 - Delete old user");
        }

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
            case "16":
                if (String.Equals(currentUser.RoleName, "Management")){
                    string newUsername;
                    string newPassword;
                    string newRole;

                    Console.WriteLine("Please input the username of the new user:");
                    newUsername = Console.ReadLine();
                    Console.WriteLine("Please input the password of the new user");
                    newPassword = Console.ReadLine();
                    Console.WriteLine("Please input the role of the new user");
                    newRole = Console.ReadLine();

                    Users newUser = SoftwareEng.UserFunctions.AddUser(newUsername, newPassword, newRole);

                    if (newUser != null)
                    {
                        Console.WriteLine("New user has been added.");
                    }
                    else
                    {
                        Console.WriteLine("Username has existed. Please try again with different username.");
                    }
                }
                break;
            case "17":
                if (String.Equals(currentUser.RoleName, "Management")){
                    string deleteUsername;

                    Console.WriteLine("Please input the username of the user you want to delete");
                    deleteUsername = Console.ReadLine();

                    bool success = SoftwareEng.UserFunctions.DeleteUser(username);

                    if (success)
                    {
                        Console.WriteLine("User has been deleted");
                    } else
                    {
                        Console.WriteLine("User is not existsed. Cannot be deleted.");
                    }
                }
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
                Console.WriteLine("Invalid input, try again\nPress Enter to continue");
                Console.ReadLine();
                break;
        }
    }


}

/*This function queries the database for employees with given username and password, if it matches one it returns true
 * and sets the global user with the credentials
 */
static bool LogInUser(string username, string password, Users currentUser)
{
    Users user = SoftwareEng.UserFunctions.ValidateUser(username, password);
    if (user == null)
    {
        return false;
    }
    else
    {
        // if we put currentUser = user, static variable will not be overwritten, hence when return from this function currentUser still remains null - soybean
        currentUser.UserID = user.UserID;
        currentUser.Username = user.Username;
        currentUser.Password = user.Password;
        currentUser.RoleName = user.RoleName;
        return true;
    }
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

Main(currentUser);