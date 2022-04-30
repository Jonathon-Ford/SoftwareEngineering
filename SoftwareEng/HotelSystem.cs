/*Authors: Jonathon Ford, Hoang Bao Duy Le, Anna Schafer
 * Professor: Lawrence Thomas
 * Class: EECS 3550 Software Engineering
 * Dates: 04/29/2022
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
 * 15.) Configure Base Rate (Set a rate for the room pricing for a certain date)
 * 16.) Create new user
 * 17.) Update old user
 * 18.) Delete old user
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
                // avoiding error when delete the password all the ways
                if(password.Length == 0) { }//Do nothing
                else
                {
                    Console.Write("\b \b");
                    password = password.Remove(password.Length - 1, 1);
                }
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

            password = null;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                // allow backspace password instead of adding new character
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length == 0) { }//Do nothing
                    else
                    {
                        Console.Write("\b \b");
                        password = password.Remove(password.Length - 1, 1);
                    }
                }
                else
                {
                    Console.Write("*");
                    password += key.KeyChar;
                }
            }
            Console.WriteLine("\n");
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
            Console.WriteLine("15 - Configure base rate");
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
                ReservationHandler.CheckAvailability();
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
                } else
                {
                    Console.WriteLine("Invaid input. Please try again");
                }
                break;
            case "13":
                if (String.Equals(currentUser.RoleName, "Management")){
                    ReportGenerator.GenerateThirtyDayIncomeReport();
                }
                else
                {
                    Console.WriteLine("Invaid input. Please try again");
                }
                break;
            case "14":
                if (String.Equals(currentUser.RoleName, "Management"))
                {
                    ReportGenerator.GenerateIncentiveReport();
                }
                else
                {
                    Console.WriteLine("Invaid input. Please try again");
                }
                break;
            case "15":
                if (String.Equals(currentUser.RoleName, "Management")){
                    ConfigureBaseRate();
                }
                else
                {
                    Console.WriteLine("Invaid input. Please try again");
                }
                break;
            case "16":
                if (String.Equals(currentUser.RoleName, "Management") || String.Equals(currentUser.RoleName, "management"))
                {
                    AddUser();
                }
                else
                {
                    Console.WriteLine("Invaid input. Please try again");
                }
                break;
            case "17":
                if (String.Equals(currentUser.RoleName, "Management") || String.Equals(currentUser.RoleName, "management"))
                {
                    UpdateUser();
                }
                else
                {
                    Console.WriteLine("Invaid input. Please try again");
                }
                break;
            case "18":
                if (String.Equals(currentUser.RoleName, "Management") || String.Equals(currentUser.RoleName, "management"))
                {
                    DeleteUser(currentUser.Username);                    
                }
                else
                {
                    Console.WriteLine("Invaid input. Please try again");
                }
                break;
            case "q":
                Console.WriteLine("Are you sure you want to log out? y/n");
                string input = Console.ReadLine();
                if (String.Equals(input, "y") || String.Equals(input, "Y"))//if they are sure do nothing
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
    // validate user if null then return to options
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
                string command;
                string email;
                int cardNum;
                string dateString;
                DateTime startDate;

                // general reservation - used when just 1 reservation under input information
                List<Reservations> reservations = SoftwareEng.PreparedStatements.FindReservation(fname, lname);

                // reseravation list filter with last 4 digits of card - used when more than 1 reservation under input information
                List<Reservations> reservationWithCard = new List<Reservations>();

                // reservation list filter with email - used when more than 1 reservation under input information
                List<Reservations> reservationWithEmail = new List<Reservations>();
                if (reservations.Count > 1)
                {
                    Console.WriteLine("Found " + reservations.Count + " reservations under your name");
                    Console.WriteLine("Please provide more details.");
                    Console.WriteLine("Please input the start date");
                    dateString = Console.ReadLine();
                    if (DateTime.TryParse(dateString, out startDate))
                    {
                        startDate = Convert.ToDateTime(dateString);
                    }
                    Console.WriteLine("Press 1 to add last 4 digits of card number. Press 2 to add email. (Default is email)");
                    command = Console.ReadLine();

                    if (String.Equals(command, "1"))
                    {
                        Console.WriteLine("Enter your card number");
                        cardNum = int.Parse(Console.ReadLine());
                        reservationWithCard = SoftwareEng.PreparedStatements.FindReservation(fname, lname, cardNum, null, startDate);
                        Console.WriteLine("\n\nRoom number: " + reservationWithCard[0].RoomNum);
                        Console.WriteLine("First name: " + reservationWithCard[0].FirstName);
                        Console.WriteLine("Last name: " + reservationWithCard[0].LastName);
                        Console.WriteLine("Email: " + reservationWithCard[0].Email);
                        Console.WriteLine("Start date: " + reservationWithCard[0].StartDate);
                        Console.WriteLine("Last date: " + reservationWithCard[0].EndDate);

                        do
                        {
                            Console.WriteLine("Is this the correct reservation? Y or N");
                            correct = Console.ReadLine();

                            // if this is a correct information, then mark this reservation as checked in 
                            if (String.Equals(correct, "y") || String.Equals(correct, "Y"))
                            {
                                for (int i = 0; i < reservationWithCard.Count; i++)
                                {
                                    SoftwareEng.PreparedStatements.MarkReservationAsCheckedIn(reservationWithCard[i]);

                                    Console.WriteLine("Successfully checked in. Enjoy your stay. Press any key to continue.");
                                    ret = Console.ReadLine();

                                    return;
                                }
                            }

                            // if not, program keeps prompting the user to input information
                            else if (String.Equals(correct, "n")  || String.Equals(correct, "N"))
                            {
                                break;
                            }
                        } while (String.Equals(correct, "n") != true || String.Equals(correct, "N") != true);
                    } else
                    {
                        Console.WriteLine("Enter your email");
                        email = Console.ReadLine();
                        reservationWithEmail = SoftwareEng.PreparedStatements.FindReservation(fname, lname, null, email, startDate);
                        Console.WriteLine("Room number: " + reservationWithEmail[0].RoomNum);
                        Console.WriteLine("First name: " + reservationWithEmail[0].FirstName);
                        Console.WriteLine("Last name: " + reservationWithEmail[0].LastName);
                        Console.WriteLine("Email: " + reservationWithEmail[0].Email);
                        Console.WriteLine("Start date: " + reservationWithEmail[0].StartDate);
                        Console.WriteLine("Last date: " + reservationWithEmail[0].EndDate);

                        do
                        {
                            Console.WriteLine("Is this the correct reservation? Y or N");
                            correct = Console.ReadLine();

                            if (String.Equals(correct, "y") || String.Equals(correct, "Y"))
                            {
                                for (int i = 0; i < reservationWithEmail.Count; i++)
                                {
                                    SoftwareEng.PreparedStatements.MarkReservationAsCheckedIn(reservationWithEmail[i]);

                                    Console.WriteLine("Successfully checked in. Enjoy your stay. Press any key to continue.");
                                    ret = Console.ReadLine();

                                    return;
                                }
                            }
                            else if (String.Equals(correct, "n")  || String.Equals(correct, "N"))
                            {
                                break;
                            }
                        } while (String.Equals(correct, "n") != true || String.Equals(correct, "N") != true);
                    }
                } else if (reservations.Count == 0)
                {
                    // if there is no reservation under the input information 
                    Console.WriteLine("There is no reservation under this information");
                    return;
                } else
                {                    
                    Console.WriteLine("Room number: " + reservations[0].RoomNum);
                    Console.WriteLine("First name: " + reservations[0].FirstName);
                    Console.WriteLine("Last name: " + reservations[0].LastName);
                    Console.WriteLine("Email: " + reservations[0].Email);
                    Console.WriteLine("Start date: " + reservations[0].StartDate);
                    Console.WriteLine("Last date: " + reservations[0].EndDate);
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
 * Pretty similar to checkin guest but instead of marking checked in, we marked check out
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
                string command;
                string email;
                int cardNum;
                string dateString;
                DateTime startDate;
                List<Reservations> reservations = PreparedStatements.FindReservation(fname, lname);
                List<Reservations> reservationWithCard = new List<Reservations>();
                List<Reservations> reservationWithEmail = new List<Reservations>();
                if (reservations.Count > 1)
                {
                    Console.WriteLine("Found " + reservations.Count + " reservations under your name");
                    Console.WriteLine("Please provide more details.");
                    Console.WriteLine("Please input the start date");
                    dateString = Console.ReadLine();
                    if (DateTime.TryParse(dateString, out startDate))
                    {
                        startDate = Convert.ToDateTime(dateString);
                    }
                    Console.WriteLine("Press 1 to add last 4 digits of card number. Press 2 to add email. (Default is email)");
                    command = Console.ReadLine();

                    if (String.Equals(command, "1"))
                    {
                        Console.WriteLine("Enter your card number");
                        cardNum = int.Parse(Console.ReadLine());
                        reservationWithCard = PreparedStatements.FindReservation(fname, lname, cardNum, null, startDate);
                        Console.WriteLine("\n\nRoom number: " + reservationWithCard[0].RoomNum);
                        Console.WriteLine("First name: " + reservationWithCard[0].FirstName);
                        Console.WriteLine("Last name: " + reservationWithCard[0].LastName);
                        Console.WriteLine("Email: " + reservationWithCard[0].Email);
                        Console.WriteLine("Start date: " + reservationWithCard[0].StartDate);
                        Console.WriteLine("Last date: " + reservationWithCard[0].EndDate);

                        do
                        {
                            Console.WriteLine("Is this the correct reservation? Y or N");
                            correct = Console.ReadLine();

                            if (String.Equals(correct, "y") || String.Equals(correct, "Y"))
                            {
                                for (int i = 0; i < reservations.Count; i++)
                                {
                                    PreparedStatements.MarkReservationAsCheckedOut(reservations[i]);

                                    ReportGenerator.GenerateBill(reservations[i]);
                                    if (!ReservationHandler.ProcessPayment("Pay bill at checkout", reservations[i]))
                                    {
                                        Console.WriteLine("Error processing bill payment");
                                    }
                                    else
                                    {
                                        reservations[i].Paid = true;
                                        reservations[i].PaymentDate = DateTime.Now.Date;
                                        PreparedStatements.UpdateReservation(reservations[i]);
                                        Console.WriteLine("Successfully checked out. Thank you for staying with us. Press any key to continue.");
                                        ret = Console.ReadLine();
                                    }

                                    return;
                                }
                            }
                            else if (String.Equals(correct, "n")  || String.Equals(correct, "N"))
                            {
                                break;
                            }
                        } while (String.Equals(correct, "n") != true || String.Equals(correct, "N") != true);
                    }
                    else
                    {
                        Console.WriteLine("Enter your email");
                        email = Console.ReadLine();
                        reservationWithEmail = SoftwareEng.PreparedStatements.FindReservation(fname, lname, null, email, startDate);
                        Console.WriteLine("Room number: " + reservationWithEmail[0].RoomNum);
                        Console.WriteLine("First name: " + reservationWithEmail[0].FirstName);
                        Console.WriteLine("Last name: " + reservationWithEmail[0].LastName);
                        Console.WriteLine("Email: " + reservationWithEmail[0].Email);
                        Console.WriteLine("Start date: " + reservationWithEmail[0].StartDate);
                        Console.WriteLine("Last date: \n\n" + reservationWithEmail[0].EndDate);

                        do
                        {
                            Console.WriteLine("Is this the correct reservation? Y or N");
                            correct = Console.ReadLine();

                            if (String.Equals(correct, "y") || String.Equals(correct, "Y"))
                            {
                                for (int i = 0; i < reservations.Count; i++)
                                {
                                    PreparedStatements.MarkReservationAsCheckedOut(reservations[i]);

                                    ReportGenerator.GenerateBill(reservations[i]);
                                    if (!ReservationHandler.ProcessPayment("Pay bill at checkout", reservations[i]))
                                    {
                                        Console.WriteLine("Error processing bill payment");
                                    }
                                    else
                                    {
                                        reservations[i].Paid = true;
                                        reservations[i].PaymentDate = DateTime.Now.Date;
                                        PreparedStatements.UpdateReservation(reservations[i]);
                                        Console.WriteLine("Successfully checked out. Thank you for staying with us. Press any key to continue.");
                                        ret = Console.ReadLine();
                                    }

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
                else if (reservations.Count == 0)
                {
                    Console.WriteLine("There is no reservation under this information");
                    return;
                }
                else
                {
                    Console.WriteLine("Room number: " + reservations[0].RoomNum);
                    Console.WriteLine("First name: " + reservations[0].FirstName);
                    Console.WriteLine("Last name: " + reservations[0].LastName);
                    Console.WriteLine("Email: " + reservations[0].Email);
                    Console.WriteLine("Start date: " + reservations[0].StartDate);
                    Console.WriteLine("Last date: \n\n" + reservations[0].EndDate);
                }

                do
                {
                    Console.WriteLine("Is this the correct reservation? Y or N");
                    correct = Console.ReadLine();

                    if (String.Equals(correct, "y") || String.Equals(correct, "Y"))
                    {
                        for (int i = 0; i < reservations.Count; i++)
                        {
                            PreparedStatements.MarkReservationAsCheckedOut(reservations[i]);

                            ReportGenerator.GenerateBill(reservations[i]);
                            if(!ReservationHandler.ProcessPayment("Pay bill at checkout", reservations[i]))
                            {
                                Console.WriteLine("Error processing bill payment");
                            }
                            else
                            {
                                reservations[i].Paid = true;
                                reservations[i].PaymentDate = DateTime.Now.Date;
                                PreparedStatements.UpdateReservation(reservations[i]);
                                Console.WriteLine("Successfully checked out. Thank you for staying with us. Press any key to continue.");
                                ret = Console.ReadLine();
                            }

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
 * It will still add user with special character
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

/*This function updates a user with provided username, password, and role
 * It will not update anything if user inputs invalid information - means that program cannot find any user with provided input
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
 * It will not delete any user if program cannot find any user with provided information
 */
static void DeleteUser(string currentUsername)
{
    string deleteUsername;

    Console.WriteLine("Please input the username of the user you want to delete");
    deleteUsername = Console.ReadLine();

    if (String.Equals(deleteUsername, currentUsername))
    {
        Console.WriteLine("Cannot delete yourself. Please try again");
        return;
    } else
    {
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
}

/*This function configures base rate
 * This will configure base rate with range - every date from effective start & end date will have provided base rate
 */
static void ConfigureBaseRate()
{
    string dateString;
    bool invalidEffectStartDate = true;
    bool invalidEffectEndDate = true;
    float baseRate;
    DateTime effectStartDate = new DateTime();
    DateTime effectiveEndDate = new DateTime();
    DateTime today = DateTime.Now;

    while (invalidEffectStartDate | invalidEffectEndDate)
    {
        Console.WriteLine("Please enter the effective start date:");
        dateString = Console.ReadLine();

        if (DateTime.TryParse(dateString, out effectStartDate))
        {
            effectStartDate = Convert.ToDateTime(dateString);
            invalidEffectStartDate = false;
        }

        Console.WriteLine("Please enter the effective end date:");
        dateString = Console.ReadLine();

        if (DateTime.TryParse(dateString, out effectiveEndDate))
        {
            effectiveEndDate = Convert.ToDateTime(dateString);
            invalidEffectEndDate = false;
        }

        try
        {
            if (today > effectStartDate || effectStartDate > effectiveEndDate)//If the start day is in the past or the end day is before the start day
            {
                invalidEffectStartDate = false; invalidEffectEndDate = false; //Fail
            }
            else
            {
                bool less = true;

                while (less)
                {
                    Console.WriteLine("Enter desired base rate");
                    baseRate = float.Parse(Console.ReadLine());

                    if (baseRate > 0)
                    {
                        for(DateTime i = effectStartDate; i <= effectiveEndDate; i = i.AddDays(1))
                        {
                            BaseRates baseRates = new BaseRates { Rate = baseRate, EffectiveDate = i, DateSet = today };
                            SoftwareEng.PreparedStatements.AddBaseRate(baseRates);
                        }
                        Console.WriteLine("The base rate(s) has been set.");
                        less = false;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not set a base rate");
            return;
        }
    }
}
/* Waits until midnight and then sets room numbers for that days arrivals and cancels 60 day
 * 
 */
static void SystemTriggered()
{

    //Wait until midngiht
    var now = DateTime.Now;
    var tomorrow = now.AddDays(1);
    var durationUntilMidnight = tomorrow.Date - now;

    var t = new Timer(o => { 

        ReportGenerator.SetRoomNumbers(); 
        ReservationHandler.ChargeNoShowFees(); 
        SystemTriggered(); 

    }, null, TimeSpan.Zero, durationUntilMidnight);

}


//PreparedStatements.PopulateWithTestData(100);
Thread atMidnight = new Thread(SystemTriggered);
while (true)
{
    Console.Clear();
    Main(currentUser);
}