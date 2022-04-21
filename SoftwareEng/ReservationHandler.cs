using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoftwareEng
{
    public static class ReservationHandler
    {
        public const int TOTAL_ROOMS = 45;

        private enum ReservationType
        {
            Prepaid = 1,
            SixtyDay = 2,
            Conventional = 3,
            Incentive = 4
        }
 
        public static void MakeReservation()
        {
            bool invalidStartDate = true, invalidEndDate = true, invalidEmail = true, full = true;
            string dateString, emailString;
            int roomsLeft;
            DateTime startDate = new DateTime(), endDate = new DateTime();
            List<int> dailyOccupancies = new List<int>();
            Reservations newReservation = new Reservations();
            newReservation.Card = new CreditCards();

            while (full)
            {
                //loop until the start date is valid
                while (invalidStartDate)
                {
                    Console.WriteLine("Please enter the potential start date:");
                    dateString = Console.ReadLine();

                    if (IsDateValid(dateString))
                    {
                        startDate = Convert.ToDateTime(dateString);
                        invalidStartDate = false;
                    }
                }

                //loop until the end date is valid
                while (invalidEndDate)
                {
                    Console.WriteLine("Please enter the potential end date:");
                    dateString = Console.ReadLine();

                    if (IsDateValid(dateString))
                    {
                        endDate = Convert.ToDateTime(dateString);

                        if (endDate > startDate)
                            invalidEndDate = false;
                        else
                            Console.WriteLine("End date must be after start date");
                    }
                }

                try
                {
                    full = false;
                    for (var i = startDate; i < endDate; i = i.AddDays(1))
                    {
                        roomsLeft = PreparedStatements.GetAvailability(i);
                        dailyOccupancies.Add(TOTAL_ROOMS - roomsLeft);

                        if (roomsLeft == 0)
                        {
                            full = true;
                            Console.WriteLine("All rooms have been booked for " + i);
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Could not retrieve availability");
                    return;
                }
            }

            newReservation.StartDate = startDate;
            newReservation.EndDate = endDate;
            newReservation.ReservationType = new ReservationTypes()
            {
                Description = DetermineReservationType(dailyOccupancies, startDate).ToString()
            };

            Console.WriteLine("Please enter the guest's information\nFirst name:");
            newReservation.FirstName = Console.ReadLine();
            Console.WriteLine("Last name:");
            newReservation.LastName = Console.ReadLine();

            while (invalidEmail)
            {
                Console.WriteLine("Email:");
                emailString = Console.ReadLine();
                if (IsEmailValid(emailString))
                {
                    newReservation.Email = emailString;
                    invalidEmail = false;
                }
                else
                    Console.WriteLine("Invalid email; please try again");
            }

            if (newReservation.ReservationType.Description != ReservationType.SixtyDay.ToString())
            {
                Console.WriteLine("Please enter payment information");
                Console.WriteLine("Credit card number:");
                newReservation.Card.CardNum = long.Parse(Console.ReadLine());
                Console.WriteLine("CVV:");
                newReservation.Card.CVVNum = int.Parse(Console.ReadLine());
                //loop until the expiration date is valid
                while (true)
                {
                    Console.WriteLine("Expiration date:");
                    dateString = Console.ReadLine();

                    if (IsDateValid(dateString))
                    {
                        newReservation.Card.ExpiryDate = Convert.ToDateTime(dateString);
                        break;
                    }
                }
            }

            try
            {
                PreparedStatements.AddReservation(newReservation);
                Console.WriteLine("Reservation added");
            }
            catch(Exception e)
            {
                Console.WriteLine("Could not add reservation");
            }
        }

        public static void EditReservation()
        {
            bool full = true, invalidStartDate = true, invalidEndDate = true;
            string dateString;
            int roomsLeft;
            List<int> dailyOccupancies = new List<int>();
            DateTime startDate = new DateTime(), endDate = new DateTime();
            var oldReservation = FindReservation();

            if (oldReservation == null)
                return;

            var newReservation = new Reservations();
            newReservation.Card = oldReservation.Card;
            newReservation.Email = oldReservation.Email;
            newReservation.FirstName = oldReservation.FirstName;
            newReservation.LastName = oldReservation.LastName;

            while (full)
            {
                //loop until the start date is valid
                while (invalidStartDate)
                {
                    Console.WriteLine("Please enter the desired new start date:");
                    dateString = Console.ReadLine();

                    if (IsDateValid(dateString))
                    {
                        startDate = Convert.ToDateTime(dateString);
                        invalidStartDate = false;
                    }
                }

                //loop until the end date is valid
                while (invalidEndDate)
                {
                    Console.WriteLine("Please enter the desired new end date:");
                    dateString = Console.ReadLine();

                    if (IsDateValid(dateString))
                    {
                        endDate = Convert.ToDateTime(dateString);
                        invalidEndDate = false;
                    }
                }

                try
                {
                    full = false;
                    for (var i = startDate; i < endDate; i = i.AddDays(1))
                    {
                        roomsLeft = PreparedStatements.GetAvailability(i);
                        dailyOccupancies.Add(TOTAL_ROOMS - roomsLeft);

                        if (roomsLeft == 0)
                        {
                            full = true;
                            Console.WriteLine("All rooms have been booked for " + i);
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Could not retrieve availability");
                }
            }

            newReservation.StartDate = startDate;
            newReservation.EndDate = endDate;
            newReservation.ReservationType = new ReservationTypes()
            {
                Description = DetermineReservationType(dailyOccupancies, startDate).ToString()
            };

            try
            {
                PreparedStatements.ChangeReservationDate(oldReservation, newReservation);
                Console.WriteLine("Reservation updated");
            }
            catch(Exception e)
            {
                Console.WriteLine("Could not update reservation");
            }
        }

        public static void CancelReservation()
        {
            try
            {
                PreparedStatements.MarkReservationAsCanceled(FindReservation());
                Console.WriteLine("Reservation cancelled");
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not cancel reservation");
            }
        }

        public static Reservations FindReservation()
        {
            string lastName, firstName, email;
            DateTime startDate;
            int cardLastFour;
            bool found = false;
            Reservations reservation = new Reservations();

            try
            {
                while (!found)
                {
                    Console.WriteLine("Please enter the first name for the reservation:");
                    firstName = Console.ReadLine();
                    Console.WriteLine("Please enter the last name for the reservation:");
                    lastName = Console.ReadLine();
                    var results = PreparedStatements.FindReservation(firstName, lastName);

                    if (results.Count == 0)
                    {
                        Console.WriteLine("Found no reservations that matched the criteria");
                        continue;
                    }
                    else if (results.Count > 1)
                    {
                        Console.WriteLine("Please enter the start date of the reservation:");
                        startDate = Convert.ToDateTime(Console.ReadLine());
                        results = PreparedStatements.FindReservation(firstName, lastName, null, null, startDate);

                        if (results.Count == 0)
                        {
                            Console.WriteLine("Found no reservations that matched the criteria");
                            continue;
                        }
                        else if (results.Count > 1)
                        {
                            Console.WriteLine("Please enter the email address with which the reservation was made");
                            email = Console.ReadLine();
                            Console.WriteLine("Please enter the last four digits of the credit card on file:");
                            cardLastFour = int.Parse(Console.ReadLine());
                            results = PreparedStatements.FindReservation(firstName, lastName, cardLastFour, email, startDate);
                        }
                    }

                    reservation = results[0];
                    Console.WriteLine($"Guest:{reservation.FirstName} {reservation.LastName} ({reservation.Email})");
                    Console.WriteLine($"Dates:{reservation.StartDate}-{reservation.EndDate}");
                    Console.WriteLine($"Credit Card:{reservation.Card.CardNum}");
                    Console.WriteLine("Is this the reservation you were looking for? Y/N");

                    if (Console.ReadLine().ToUpper() == "Y")
                    {
                        found = true;
                    }
                }

                return reservation;
            }
            catch(Exception e)
            {
                Console.WriteLine("Error searching reservations");
                return null;
            }
        }

        public static void ConfirmReservation()
        {
            try
            {
                var reservation = FindReservation();
                reservation.Confirmed = true;
                PreparedStatements.UpdateReservation(reservation);
                Console.WriteLine("Reservation confirmed");
            }
            catch(Exception e)
            {
                Console.WriteLine("Could not confirm reservation");
            }
        }

        //public static void CheckAvailability()
        //{

        //}

        private static ReservationType DetermineReservationType(List<int> dailyOccupancies, DateTime startDate)
        {
            var daysOut = (startDate - DateTime.Now).Days;

            if(daysOut >= 90)
            {
                return ReservationType.Prepaid;
            }
            else if(daysOut >= 60)
            {
                return ReservationType.SixtyDay;
            }
            else if(daysOut >= 30 && dailyOccupancies.Average()/TOTAL_ROOMS > 0.6)
            {
                return ReservationType.Conventional;
            }
            else
            {
                return ReservationType.Incentive;
            }
        }

        private static bool IsDateValid(string input)
        {
            try
            {
                var date = Convert.ToDateTime(input);
                var curDate = DateTime.Now;

                if (date >= curDate)
                    return true;
                else
                {
                    Console.WriteLine($"Date cannot be before {curDate.Month}/{curDate.Day}/{curDate.Year}");
                    return false;
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine("Invalid date; please try again.");
                return false;
            }
        }

        private static bool IsEmailValid(string input)
        {
            Regex emailPattern = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            try
            {
                return emailPattern.Match(input).Success;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
