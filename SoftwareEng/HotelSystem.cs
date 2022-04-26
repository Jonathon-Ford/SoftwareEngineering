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

using SoftwareEng;
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
        //Hidden read
        password = null;
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
                break;
            else if (key.Key == ConsoleKey.Backspace)
            {
                Console.Write("\b \b");
                password = password.Remove(password.Length - 1, 1);
            }
            else
            {
                Console.Write("*");
                password += key.KeyChar;
            }
        }

        Console.WriteLine("\nChecking credentials...");
        while (!LogInUser(username, password, currentUser) && remainingAttempts > 0)//if the user is not logged in and they have remaining attempts
        {
            Console.WriteLine("Invalid log in, try again");
            Console.WriteLine("Please input your username:");
            username = Console.ReadLine();
            Console.WriteLine("Please input your password");
            System.Console.Write("password: ");

            password = null;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                else if (key.Key == ConsoleKey.Backspace)
                {
                    Console.Write("\b \b");
                    password = password.Remove(password.Length - 1, 1);
                }
                else
                {
                    Console.Write("*");
                    password += key.KeyChar;
                }
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
    int numCommands = 11;
    if(currentUser.RoleName == "Management")
    {
        numCommands = 18;
    }
    while (!String.Equals(command, "q"))//Until the user logs out, print the options and read what they would like to do
    {
        Console.WriteLine("Input your desired function (1 - "+ numCommands +") or q to log out:");
        Console.WriteLine("1  - Make a reservation");
        Console.WriteLine("2  - Edit a reservation");
        Console.WriteLine("3  - Cancel a reservation");
        Console.WriteLine("4  - Check reservation details");
        Console.WriteLine("5  - Confirm reservation");
        Console.WriteLine("6  - Check availability");
        Console.WriteLine("7  - Check in guest");
        Console.WriteLine("8  - Check out guest");
        Console.WriteLine("9  - Generate daily emails");
        Console.WriteLine("10 - Generate daily arivals report");
        Console.WriteLine("11 - Generate daily occupancy report");
        if (String.Equals(currentUser.RoleName, "Management") || (String.Equals(currentUser.RoleName, "management"))){
            Console.WriteLine("12 - Generate 30 day occupancy report");
            Console.WriteLine("13 - Generate 30 day expected income report");
            Console.WriteLine("14 - Generate 30 day incentive loss report");
            Console.WriteLine("15 - Set base rate");
            Console.WriteLine("16 - Create new user");
            Console.WriteLine("17 - Update old user");
            Console.WriteLine("18 - Delete old user");
        }

        command = Console.ReadLine();
        switch (command)
        {
            case "1":
                ReservationHandler.MakeReservation();
                break;
            case "2":
                ReservationHandler.EditReservation();
                break;
            case "3":
                ReservationHandler.CancelReservation();
                break;
            case "4":
                ReservationHandler.FindReservation();
                break;
            case "5":
                ReservationHandler.ConfirmReservation();
                break;
            case "6":
                CheckAvailability();
                break;
            case "7":
                CheckInGuest();
                break;
            case "8":
                CheckOutGuest();
                break;
            case "9":
                Email.GenerateDailyEmails();
                break;
            case "10":
                ReportGenerator.GenerateDailyArrivalsReport();
                break;
            case "11":
                ReportGenerator.GenerateDailyOccupancyReport();
                break;
            case "12":
                if (String.Equals(currentUser.RoleName, "Management") || String.Equals(currentUser.RoleName, "management"))
                {
                    ReportGenerator.GenerateThirtyDayOccupancyReport();
                }
                break;
            case "13":
                if (String.Equals(currentUser.RoleName, "Management")){
                    ReportGenerator.GenerateThirtyDayIncomeReport();
                }
                break;
            case "14":
                if (String.Equals(currentUser.RoleName, "Management"))
                {
                    ReportGenerator.GenerateIncentiveReport();
                }
                break;
            case "15":
                //if(String.Equals(currentUser.role, "Management"){
                //  GenerateExpectedOccupancyReport();
                //}
                break;
            case "16":
                if (String.Equals(currentUser.RoleName, "Management") || String.Equals(currentUser.RoleName, "management"))
                {
                    AddUser();
                }
                break;
            case "17":
                if (String.Equals(currentUser.RoleName, "Management") || String.Equals(currentUser.RoleName, "management"))
                {
                    UpdateUser();
                }
                break;
            case "18":
                if (String.Equals(currentUser.RoleName, "Management") || String.Equals(currentUser.RoleName, "management"))
                {
                    DeleteUser();                    
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
    string fname;
    string lname;
    string correct;
    string ret;

    do
    {
        Console.WriteLine("Input guest first name or press q to return to options");
        fname = Console.ReadLine();

        if (String.Equals(fname, "q") || String.Equals(fname, "Q"))
        {
            return;
        }
        else
        {

            Console.WriteLine("Input guest last name or press q to return to options");
            lname = Console.ReadLine();

            if (String.Equals(lname, "q") || String.Equals(lname, "Q"))
            {
                return;
            }
            else
            {
                List<Reservations> reservations = SoftwareEng.PreparedStatements.FindReservation(fname, lname);

                for (int i = 0; i < reservations.Count; i++)
                {
                    Console.WriteLine("Reservation " + i);
                    Console.WriteLine("Room number: " + reservations[i].RoomNum);
                    Console.WriteLine("First name: " + reservations[i].FirstName);
                    Console.WriteLine("Last name: " + reservations[i].LastName);
                    Console.WriteLine("Email: " + reservations[i].Email);
                    Console.WriteLine("Start date: " + reservations[i].StartDate);
                    Console.WriteLine("Last date: " + reservations[i].EndDate);
                }

                do
                {
                    Console.WriteLine("Is this the correct reservation? Y or N");
                    correct = Console.ReadLine();

                    if (String.Equals(correct, "y") || String.Equals(correct, "Y"))
                    {
                        for (int i = 0; i < reservations.Count; i++)
                        {
                            SoftwareEng.PreparedStatements.MarkReservationAsCheckedIn(reservations[i]);

                            Console.WriteLine("Successfully checked in. Enjoy your stay. Press any key to continue.");
                            ret = Console.ReadLine();

                            return;
                        }
                    } else if (String.Equals(correct, "n")  || String.Equals(correct, "N"))
                    {
                        break;
                    }
                } while (String.Equals(correct, "n") != true || String.Equals(correct, "N") != true);                  
            }
        }
    } while (String.Equals(correct, "n") || String.Equals(correct, "N"));
}
/*This function checks out a guest at the end of their stay
 * 
 */
static void CheckOutGuest()
{
    string fname;
    string lname;
    string correct;
    string ret;

    do
    {
        Console.WriteLine("Input guest first name or press q to return to options");
        fname = Console.ReadLine();

        if (String.Equals(fname, "q") || String.Equals(fname, "Q"))
        {
            return;
        }
        else
        {

            Console.WriteLine("Input guest last name or press q to return to options");
            lname = Console.ReadLine();

            if (String.Equals(lname, "q") || String.Equals(lname, "Q"))
            {
                return;
            }
            else
            {
                List<Reservations> reservations = SoftwareEng.PreparedStatements.FindReservation(fname, lname);

                for (int i = 0; i < reservations.Count; i++)
                {
                    Console.WriteLine("Reservation " + i);
                    Console.WriteLine("Room number: " + reservations[i].RoomNum);
                    Console.WriteLine("Room number: " + reservations[i].ReservationType);
                    Console.WriteLine("First name: " + reservations[i].FirstName);
                    Console.WriteLine("Last name: " + reservations[i].LastName);
                    Console.WriteLine("Email: " + reservations[i].Email);
                    Console.WriteLine("Start date: " + reservations[i].StartDate);
                    Console.WriteLine("Last date: " + reservations[i].EndDate);
                    Console.WriteLine("Total price: " + reservations[i].Price);
                }

                do
                {
                    Console.WriteLine("Is this the correct reservation? Y or N");
                    correct = Console.ReadLine();

                    if (String.Equals(correct, "y") || String.Equals(correct, "Y"))
                    {
                        for (int i = 0; i < reservations.Count; i++)
                        {
                            SoftwareEng.PreparedStatements.MarkReservationAsCheckedOut(reservations[i]);

                            // generate bill here

                            return;
                        }
                    }
                    else if (String.Equals(correct, "n")  || String.Equals(correct, "N"))
                    {
                        break;
                    }
                } while (String.Equals(correct, "n") != true || String.Equals(correct, "N") != true);
            }
        }
    } while (String.Equals(correct, "n") || String.Equals(correct, "N"));
}
/*This function adds a user with provided username, password, and role
 * 
 */
static void AddUser()
{
    string newUsername;
    string newPassword;
    string newRole;

    Console.WriteLine("Please input the username of the new user:");
    newUsername = Console.ReadLine();
    Console.WriteLine("Please input the password of the new user");
    newPassword = Console.ReadLine();
    Console.WriteLine("Please input the role of the new user (Employee or Management)");
    newRole = Console.ReadLine();

     while (String.Equals(newRole, "Employee") != true && String.Equals(newRole, "Management") != true && String.Equals(newRole, "employee") != true && String.Equals(newRole, "management") != true)
     {
        Console.WriteLine("Please input the role of the new user (Employee or Management)");
        newRole = Console.ReadLine();
     }
    

    Users newUser = SoftwareEng.UserFunctions.AddUser(newUsername, newPassword, newRole);

    if (newUser != null)
    {
        Console.WriteLine("New user has been added.\n");
    }
    else
    {
        Console.WriteLine("Username has existed. Please try again with different username.\n");
    }
}
/*This function adds a user with provided username, password, and role
 * 
 */
static void CheckAvailability()
{
    DateTime now = DateTime.Now;
    int emptyRoom = SoftwareEng.PreparedStatements.GetAvailability(now);
    Console.WriteLine(emptyRoom);
}
/*This function updates a user with provided username, password, and role
 * 
 */
static void UpdateUser()
{
    string newUsername;
    string newPassword;
    string newRole;
    string command;
    string oldUsername;

    
    Console.WriteLine("Please input the username of the user you want to update or press q to return to option");
    oldUsername = Console.ReadLine();

    if (String.Equals(oldUsername, "q") || String.Equals(oldUsername, "Q"))
    {
        return;
    }
    else
    {
        Users oldUser = SoftwareEng.UserFunctions.FindUser(oldUsername);

        if (oldUser != null)
        {
            Console.WriteLine("Please input the new username");
            newUsername = Console.ReadLine();
            Console.WriteLine("Please input the new password");
            newPassword = Console.ReadLine();
            Console.WriteLine("Please input the new role (Employee or Management)");
            newRole = Console.ReadLine();

            while (String.Equals(newRole, "Employee") != true && String.Equals(newRole, "Management") != true && String.Equals(newRole, "employee") != true && String.Equals(newRole, "management") != true)
            {
                Console.WriteLine("Please input the new role (Employee or Management)");
                newRole = Console.ReadLine();
            }

            bool success = UserFunctions.UpdateUser(oldUsername, newUsername, newPassword, newRole);

            if (success == true)
            {
                Console.WriteLine("User has been updated");
            }
            else
            {
                Console.WriteLine("There is a problem. Please try again");
            }
        }
        else
        {
            Console.WriteLine("Username is not existed\n");
        }
    }

    
}
/*This function deletes a user with provided username
 * 
 */
static void DeleteUser()
{
    string deleteUsername;

    Console.WriteLine("Please input the username of the user you want to delete");
    deleteUsername = Console.ReadLine();

    bool success = SoftwareEng.UserFunctions.DeleteUser(deleteUsername);

    if (success)
    {
        Console.WriteLine("User has been deleted");
    }
    else
    {
        Console.WriteLine("User not found. Cannot be deleted.");
    }
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
/*
 * 
 */
static void SystemTriggered()
{
    while (true)
    {
        //Wait until midngiht
        var now = DateTime.Now;
        var tomorrow = now.AddDays(1);
        var durationUntilMidnight = tomorrow.Date - now;

        var t = new Timer(o => {/* Do work*/}, null, TimeSpan.Zero, durationUntilMidnight);

        ReportGenerator.SetRoomNumbers();
    }
}

Thread atMidnight = new Thread(SystemTriggered);
Main(currentUser);