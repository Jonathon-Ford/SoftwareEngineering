using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoftwareEng.DataModels;

namespace SoftwareEng
{
    public class PreparedStatements
    {
        public static int errno = 0;
        /* This function finds a reservation with 3 levels of specificity
         * First it will look for reservations with the desired name, if they have more than one res it will try to specify the search
         * with the optional parameters of card num / email and start date
         */
        public static List<Reservations> FindReservation(string FName, string LName, int? lastFourOfCard = null, string? email = null, DateTime startDate = default(DateTime))
        {
            using DatabaseContext db = new DatabaseContext();
            var curReservations = db
                .Reservations
                .Where(r => r.FirstName == FName)
                .Where(r => r.LastName == LName)
                .ToList();

            if(curReservations.Count > 1)
            {
                if(lastFourOfCard != null)
                {
                    if (startDate == default(DateTime))//If only have last four and no start date
                    {
                        curReservations = db
                            .Reservations
                            .Where(r => r.FirstName == FName)
                            .Where(r => r.LastName == LName)
                            .Where(r => r.Card.CardNum % 10000 == lastFourOfCard)
                            .ToList();
                    }
                    else //If both last four and start date
                    {
                        curReservations = db
                            .Reservations
                            .Where(r => r.FirstName == FName)
                            .Where(r => r.LastName == LName)
                            .Where(r => r.Card.CardNum % 10000 == lastFourOfCard)
                            .Where(r => r.StartDate.Date == startDate.Date)
                            .ToList();
                    }
                }
                else if(email != null)
                {
                    if(startDate == default(DateTime))
                    {
                        curReservations = db
                        .Reservations
                        .Where(r => r.FirstName == FName)
                        .Where(r => r.LastName == LName)
                        .Where(r => r.Email == email)
                        .ToList();
                    }
                    else
                    {
                        curReservations = db
                        .Reservations
                        .Where(r => r.FirstName == FName)
                        .Where(r => r.LastName == LName)
                        .Where(r => r.Email == email)
                        .Where(r => r.StartDate.Date == startDate.Date)
                        .ToList();
                    }
                }
            }
            return curReservations;
        }
        /* This function gets a list for all of the base rates for each day of a stay
         * 
         * 
         */
        public static List<float> GetBaseRates(DateTime startDate, DateTime endDate)
        {
            using DatabaseContext db = new DatabaseContext();
            List<float> rates = new List<float>();

            for (var day = startDate; day <= endDate; day.AddDays(1))
            {
                var curPrice = db
                    .BaseRates
                    .Where(br => br.EffectiveDate.Date == day.Date)
                    .OrderByDescending(br => br.DateSet)
                    .First();

                rates.Add(curPrice.Rate);
            }
            return rates;
        }
        /* This function updates a reservation record
         *
         * Takes in the edited reservation (note you must have found the reservation with the correct key) 
         */
        public static void UpdateReservation(Reservations editedReso)
        {
            using DatabaseContext db = new DatabaseContext();
            db.Entry(editedReso).State = EntityState.Modified;
            db.SaveChanges();
        }
        /* This function adds a reservation to the reservation table
         * Takes: the reservation to add
         */
        public static void AddReservation(Reservations resoToAdd)
        {
            using DatabaseContext db = new DatabaseContext();
            db.Reservations.Add(resoToAdd);
            db.SaveChanges();
        }
        /* This function marks the reservation given as canceled and sets the date it was canceled to the current day
         * 
         * 
         */
        public static void MarkReservationAsCanceled(Reservations toCancel)
        {
            toCancel.IsCanceled = true;
            toCancel.DateCanceled = DateTime.Now;

            using DatabaseContext db = new DatabaseContext();
            db.Entry(toCancel).State = EntityState.Modified;
            db.SaveChanges();
        }
        /* Attempts to send back a user if the username and password match a record in the database, if not it returns an empty user class
         * and sets the errno to 1
         */
        public static Users ValidateUser(string username, string password)
        {
            using DatabaseContext db = new DatabaseContext();

            var user = db
                .Users
                .Where(u => u.Username == username)
                .Where(u => u.Password == password)
                .SingleOrDefault();

            if(user == null)
            {
                errno = 1;
                return new Users(); //Return an empty user class to show it was not found
            }
            errno = 0;
            return user; //Return the correct user
        }
        /* This function updates the database so a reservation is checked in
         * 
         */
        public static void MarkReservationAsCheckedIn(Reservations toCheckIn)
        {
            toCheckIn.CheckedIn = true;

            using DatabaseContext db = new DatabaseContext();
            db.Entry(toCheckIn).State = EntityState.Modified;
            db.SaveChanges();
        }
        /* This function updates the database so a reservation is checked out
         * 
         */
        public static void MarkReservationAsCheckedOut(Reservations toCheckOut)
        {
            toCheckOut.CheckedOut = true;

            using DatabaseContext db = new DatabaseContext();
            db.Entry(toCheckOut).State = EntityState.Modified;
            db.SaveChanges();
        }
        /* This function gives you the availability for the day
         * 
         */
        public static int GetAvailability(DateTime day)
        {
            using DatabaseContext db = new DatabaseContext();
            var count = db
                .Reservations
                .Where(r => r.StartDate.Date <= day.Date)
                .Where(r => r.EndDate.Date >= day.Date)
                .Count();

            return 45 - count;
        }
        /*
         * 
         */
        public static List<Reservations> GetReservationsForEmail()
        {
            using DatabaseContext db = new DatabaseContext();
            var toEmailList = db
                .Reservations
                .Where(r => r.ReservationType.ReservationID == 2) //2 is for 60 day reservations
                .Where(r => r.Paid == false)
                .Where(r => (r.StartDate - DateTime.Now).Days <= 45)
                .ToList();
            return toEmailList;
        }
        /* Returns a list of reservations that are expected to arrive today
         * 
         */
        public static List<Reservations> GetDailyArrivals()
        {
            using DatabaseContext db = new DatabaseContext();
            var dailyArrivals = db
                .Reservations
                .Where(r => r.StartDate.Date == DateTime.Now.Date)
                .ToList();

            var lateArrivals = db
                .Reservations
                .Where(r => r.Confirmed == true)
                .Where(r => r.StartDate.Date < DateTime.Now.Date)
                .Where(r => r.EndDate.Date >= DateTime.Now.Date)
                .Where(r => r.CheckedIn == false)
                .Where(r => r.IsCanceled == false)
                .ToList();

            return dailyArrivals.Concat(lateArrivals).ToList();
        }
        /*
         * 
         */
        public static List<Reservations> GetTodaysOccupancies()
        {
            using DatabaseContext db = new DatabaseContext();
            var todaysOccupancies = db
                .Reservations
                .Where(r => r.CheckedIn == true)
                .Where(r => r.CheckedOut == false)
                .OrderBy(r => r.RoomNum)
                .ToList();
            return todaysOccupancies;
        }
        /* returns a list of a list of a list of reservations in the form:
         * 
         * object[day][reservation_type - 1] = count 
         * 
         */
        public static List<List<int>> GetThirtyDayOccupancyInfo()
        {
            List<List<int>> occupancyInfo = new List<List<int>>();
            using DatabaseContext db = new DatabaseContext();

            DateTime curDate = DateTime.Now;
            for(int i = 0; i < 30; i++)
            {
                List<int> resoCount = new List<int>();

                for(int j = 1; j <= 4; j++)
                {
                    var count = db
                        .Reservations
                        .Where(r => r.StartDate.Date <= curDate.Date)
                        .Where(r => r.EndDate.Date >= curDate.Date)
                        .Where(r => r.ReservationType.ReservationID == i)
                        .Count();
                    resoCount.Add(count);
                }
                occupancyInfo.Add(resoCount);
                curDate.AddDays(1);
            }

            return occupancyInfo;
        }
        /*
         * 
         *
        public static List<int> GetThirtyDayIncomeInfo()
        {
            List<int> incomeList = new List<int>(30);
            using DatabaseContext db = new DatabaseContext();

            DateTime curDate = DateTime.Now;
            for(int i = 0; i < 30; i++)
            {
                var income =
                    (
                    from br in db.BaseRates
                    join dr in db.DayRates
                    where dr.Rates.Where(e => e.Rate == br.Rate)
                        )

            }
        }
        */
        public static void AddReservationTest()
        {
            using DatabaseContext db = new DatabaseContext();

            BaseRates br1 = new BaseRates()
            {
                BaseRateID = 1,
                Rate = 10,
                EffectiveDate = DateTime.Now.Date,
                DateSet = DateTime.Now,
            };
            BaseRates br2 = new BaseRates()
            {
                BaseRateID = 2,
                Rate = 10,
                EffectiveDate = DateTime.Now.AddDays(1).Date,
                DateSet = DateTime.Now,
            };

            db.BaseRates.Add(br1);
            db.BaseRates.Add(br2);

            List<BaseRates> baseRates = new List<BaseRates>() { br1, br2 };

            Reservations reso = new Reservations()
            {
                ReservationID = 1,
                LastName = "Bob",
                FirstName = "Billy",
                Email = "ThisIsFake@Fake.com",
                ReservationType = new ReservationTypes() { ReservationID = 1, PercentOfBase = (float).8 },
                Price = 20,
                RoomNum = 1,
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.AddDays(2).Date,
                IsCanceled = false,
                Paid = true,
                PaymentDate = DateTime.Now.Date,
                Confirmed = false,
                CheckedIn = true,
                CheckedOut = false,
                BaseRates = baseRates
            };

            db.Reservations.Add(reso);

            var rates = db
                .Reservations
                .Where(r => r.ReservationID == 1)
                .First();

            for(int i = 0; i < rates.BaseRates.Count; i++)
            {
                Console.WriteLine("Rate " + i + " : " + rates.BaseRates.ToList()[i]);
            }

        }
    }
}
