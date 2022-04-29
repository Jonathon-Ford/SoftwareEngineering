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

        public enum ReservationTypeCode
        {
            Prepaid = 1,
            SixtyDay = 2,
            Conventional = 3,
            Incentive = 4
        }
 
        public static void MakeReservation()
        {
            bool invalidStartDate = true, invalidEndDate = true, invalidEmail = true, invalidcardNum = true, invalidCvv = true, full = true;
            string dateString, emailString, cardNumString, cvvString;
            long cardNum;
            int roomsLeft, cvv;
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
            newReservation.ReservationType = DetermineReservationType(dailyOccupancies, startDate);

            try
            {
            newReservation.BaseRates = PreparedStatements.GetBaseRates(startDate, endDate).ToList();
            newReservation.Price = CalculateReservationPrice(newReservation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            if (newReservation.Price == 0)
                return;

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

            if (newReservation.ReservationType.Description != ReservationTypeCode.SixtyDay.ToString())
            {
                //loop until the card information is valid
                while (true)
                {
                    Console.WriteLine("Please enter payment information");

                    while (true)
                    {
                        Console.WriteLine("Credit card number (XXXXXXXXXXXXXXXX):");
                        cardNumString = Console.ReadLine().Trim();

                        if (cardNumString.Length == 16 && long.TryParse(cardNumString, out cardNum))
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input for card number; please try again");
                        }
                    }

                    while (true)
                    {
                        Console.WriteLine("CVV (XXX or XXXX):");
                        cvvString = Console.ReadLine().Trim();

                        if((cvvString.Length == 3 || cvvString.Length == 4) && int.TryParse(cvvString, out cvv))
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input for CVV; please try again");
                        }
                    }

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

                    if(!IsCardValid(cardNum, cvv, newReservation.Card.ExpiryDate))
                    {
                        Console.WriteLine("No card found with the given information; please try again");
                    }
                    else
                    {
                        newReservation.Card.CardNum = cardNum;
                        newReservation.Card.CVVNum = cvv;
                        break;
                    }
                }
            }

            try
            {
                PreparedStatements.AddReservation(newReservation);
                Console.WriteLine("Reservation added");

                if (newReservation.ReservationType.Description == ReservationTypeCode.Prepaid.ToString())
                {

                    if (!ProcessPayment("Pay bill when reservation made", newReservation))
                    {
                        Console.WriteLine("Error processing payment");
                        return;
                    }
                    else
                    {
                        newReservation.Paid = true;
                        newReservation.PaymentDate = DateTime.Now.Date;
                        PreparedStatements.UpdateReservation(newReservation);
                        Console.WriteLine("Reservation paid");
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error making reservation");
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
                var reservation = FindReservation();

                if((reservation.StartDate.Date - DateTime.Now.Date).Days < 3)
                {
                    if (reservation.ReservationType.Description == ReservationTypeCode.Conventional.ToString()
                        || reservation.ReservationType.Description == ReservationTypeCode.Incentive.ToString())
                    {
                        reservation.Price = CalculateFirstDayPrice(reservation);
                        if(!ProcessPayment("Charge for cancelling w/in 3 days", reservation))
                        {
                            Console.WriteLine("Error cancelling reservation: could not charge cancellation fee");
                            return;
                        }
                        else
                        {
                            reservation.Paid = true;
                            reservation.PaymentDate = DateTime.Now.Date;
                            PreparedStatements.UpdateReservation(reservation);
                        }
                    }
                }

                PreparedStatements.MarkReservationAsCanceled(reservation);
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

        /// <summary>
        /// This funnction accepts a reservation or finds a reservation and "charges" the card associated with it.
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="paymentDescription"></param>
        /// <returns>bool indicating success or failure</returns>
        public static bool ProcessPayment(string paymentDescription, Reservations reservation = null)
        {
            var payment = new Payments();

            try
            {
                if (reservation == null)
                {
                    reservation = ReservationHandler.FindReservation();
                }

                payment.Reservation = reservation ?? throw new Exception("");
                payment.PaymentDate = DateTime.Now.Date;
                payment.Card = reservation.Card;
                payment.Description = paymentDescription;
                payment.Price = reservation.Price;

                try
                {
                    PreparedStatements.AddPayment(payment);
                    return true;
                }
                catch (Exception exception)
                {
                    throw new Exception("Could not add payment");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static void ChargeNoShowFees()
        {
            var reservations = PreparedStatements.GetNoShowReservations();

            foreach (var res in reservations)
            {
                res.Price = CalculateFirstDayPrice(res);
                ProcessPayment("No-show fee", res);
            }
        }

        private static ReservationTypes DetermineReservationType(List<int> dailyOccupancies, DateTime startDate)
        {
            var daysOut = (startDate - DateTime.Now).Days;
            var type = new ReservationTypes();

            if(daysOut >= 90)
            {
                type.ReservationID = (int)ReservationTypeCode.Prepaid;
                type.Description = ReservationTypeCode.Prepaid.ToString();
                return type;
            }
            else if(daysOut >= 60)
            {
                type.ReservationID = (int)ReservationTypeCode.SixtyDay;
                type.Description = ReservationTypeCode.SixtyDay.ToString();
                return type;
            }
            else if(daysOut >= 30 && dailyOccupancies.Average()/TOTAL_ROOMS > 0.6)
            {
                type.ReservationID = (int)ReservationTypeCode.Conventional;
                type.Description = ReservationTypeCode.Conventional.ToString();
                return type;
            }
            else
            {
                type.ReservationID = (int)ReservationTypeCode.Incentive;
                type.Description = ReservationTypeCode.Incentive.ToString();
                return type;
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

        private static bool IsCardValid(long cardNum, int cvv, DateTime expiration)
        {
            try
            {
                CreditCards possibleCard = new CreditCards()
                {
                    CardNum = cardNum,
                    CVVNum = cvv,
                    ExpiryDate = expiration
                };
                CreditCards cardResult;
            
                cardResult = PreparedStatements.FindCardByNum(possibleCard);

                if (cardResult != null && cardResult.CVVNum == possibleCard.CVVNum && cardResult.ExpiryDate == possibleCard.ExpiryDate)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private static double CalculateReservationPrice(Reservations reservation)
        {
            try
            {
                var sum = 0.0;
                var rates = reservation.BaseRates.ToList();

                var reservationTypeInfo = PreparedStatements.GetReservationTypeDetails(reservation.ReservationType);

                //Multiply each element by the percentage determined by the reservation type
                foreach (var rate in rates)
                {
                    sum += rate.Rate * reservationTypeInfo.PercentOfBase / 100;
                }

                //Return the sum of all the daily rates
                return sum;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        private static double CalculateFirstDayPrice(Reservations reservation)
        {
            var nextDay = reservation.StartDate.AddDays(1);
            var firstBaseRate = PreparedStatements.GetBaseRates(reservation.StartDate, nextDay).First();
            return firstBaseRate.Rate * PreparedStatements.GetReservationTypeDetails(reservation.ReservationType).PercentOfBase / 100;
        }
    }
}
