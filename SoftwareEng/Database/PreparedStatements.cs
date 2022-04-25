/* Author Jonathon Ford
 * 
 * This Class contains all comunication between the database and the code
 * You can add and remove data using only these functions to insure that faulty data does not get put in the system
 * 
 * 
 * 
 */

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

        //******HOTEL SYSTEM STATEMENTS***********************************************************

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

            if (user == null)
            {
                errno = 1;
                return user; //Return an empty user class to show it was not found
            }
            errno = 0;
            return user; //Return the correct user
        }

        /* Attempts to find a user if the username and password match a record in the database, if not it returns an empty user class
         * and sets the errno to 1
         * Borrow code from ValidateUser but not get password parameter because users could have same password but not same username
         */
        public static Users FindUser(string username)
        {
            using DatabaseContext db = new DatabaseContext();

            var user = db
                .Users
                .Where(u => u.Username == username)
                .SingleOrDefault();

            if (user == null)
            {
                errno = 0;
                return user; //Return an empty user class to show it was not found
            }
            errno = 1;
            return user;
        }

        /* Add a user with username, password, and role
         */
        public static Users AddUser(String username, String password, String role)
        {
            using DatabaseContext db = new DatabaseContext();
            Users newUser = new Users { Username = username, Password = password, RoleName = role };
            db.Users.Add(newUser);
            db.SaveChanges();

            return newUser;
        }

        /* Update a user with username, password, and role
         */
        public static Users UpdateUser(String oldUsername, String username, String password, String role)
        {
            using DatabaseContext db = new DatabaseContext();

            var user = db
                .Users
                .Where(u => u.Username == oldUsername)
                .SingleOrDefault();

            user.Username = username;
            user.Password = password;
            user.RoleName = role;
            db.SaveChanges();
            return user;
        }

        /* Delete a user with username, password, and role
         */
        public static bool DeleteUser(String username)
        {
            using DatabaseContext db = new DatabaseContext();

            var user = db
                .Users
                .Where(u => u.Username == username)
                .SingleOrDefault();

            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        /* Adds a new base rate, if there is already a base rate for that day it is ok, this will be used in the future
         * 
         */
        public static void AddBaseRate(BaseRates baserate)
        {
            using DatabaseContext db = new DatabaseContext();
            db.BaseRates.Add(baserate);
            db.SaveChanges();
        }

        /* This function updates the database so a reservation is checked in
         * 
         */
        public static void MarkReservationAsCheckedIn(Reservations toCheckIn)
        {
            toCheckIn.CheckedIn = true;

            using DatabaseContext db = new DatabaseContext();
            db.Entry(toCheckIn).State = EntityState.Modified;
            db.Entry(toCheckIn.Card).State = EntityState.Unchanged;
            db.Entry(toCheckIn.ReservationType).State = EntityState.Unchanged;
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
            db.Entry(toCheckOut.Card).State = EntityState.Unchanged;
            db.Entry(toCheckOut.ReservationType).State = EntityState.Unchanged;
            db.SaveChanges();
        }

        //******RESERVATION STATEMENTS************************************************************

        /* This function finds a reservation with 3 levels of specificity
         * First it will look for reservations with the desired name, if they have more than one res it will try to specify the search
         * with the optional parameters of card num / email and start date
         */
        public static List<Reservations> FindReservation(string FName, string LName, int? lastFourOfCard = null, string? email = default, DateTime startDate = default(DateTime))
        {
            using DatabaseContext db = new DatabaseContext();
            var curReservations = db
                .Reservations
                .Include("Card")
                .Include("ReservationType")
                .Where(r => r.FirstName == FName)
                .Where(r => r.LastName == LName)
                .ToList();

            if(curReservations.Count > 1)
            {
                if(lastFourOfCard != null)
                {
                    if (startDate == default(DateTime))//If only have last four and no start date
                    {
                        curReservations = curReservations
                            .Where(r => r.Card.CardNum % 10000 == lastFourOfCard)
                            .ToList();
                    }
                    else //If both last four and start date
                    {
                        curReservations = curReservations
                            .Where(r => r.Card.CardNum % 10000 == lastFourOfCard)
                            .Where(r => r.StartDate == startDate)
                            .ToList();
                    }
                }
                else if(email != null)
                {
                    if(startDate == default(DateTime))
                    {
                        curReservations = curReservations
                        .Where(r => r.Email == email)
                        .ToList();
                    }
                    else
                    {
                        curReservations = curReservations
                        .Where(r => r.Email == email)
                        .Where(r => r.StartDate == startDate)
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
        public static List<BaseRates> GetBaseRates(DateTime startDate, DateTime endDate)
        {
            using DatabaseContext db = new DatabaseContext();
            List<BaseRates> rates = new List<BaseRates>();

            for (var day = startDate.Date; day < endDate.Date; day.AddDays(1))
            {
                var curPrice = db
                    .BaseRates
                    .Where(br => br.EffectiveDate.Date == day.Date)
                    .OrderByDescending(br => br.DateSet)
                    .First();

                rates.Add(curPrice);
            }
            return rates;
        }

        /* This function updates a reservation record
         *
         * Takes in the edited reservation (note you must have found the reservation with the correct key) 
         * 
         * *****WARNING : You should not change the date with this function, use change reservation date func****
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

            resoToAdd.ReservationType = db.ReservationTypes.Where(rt => rt.Description == resoToAdd.ReservationType.Description).First();
            db.Reservations.Add(resoToAdd);
            db.Entry(resoToAdd.Card).State = EntityState.Unchanged;
            db.Entry(resoToAdd.ReservationType).State = EntityState.Unchanged;
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
            db.Entry(toCancel.Card).State = EntityState.Unchanged;
            db.Entry(toCancel.ReservationType).State = EntityState.Unchanged;
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
                .Where(r => r.StartDate <= day)
                .Where(r => r.EndDate >= day)
                .Where(r => r.IsCanceled == false)
                .Count();

            return 45 - count;
        }

        /* Marks a reservation as being changed to the new reso
         * 
         * Takes: The old reservation (this will ensure it is canceled)
         *        A new reservation
         */
        public static void ChangeReservationDate(Reservations oldReso, Reservations newReso)
        {
            MarkReservationAsCanceled(oldReso);
            AddReservation(newReso);

            using DatabaseContext db = new DatabaseContext();
            db.Entry(oldReso).State = EntityState.Unchanged;
            db.Entry(newReso).State = EntityState.Unchanged;

            db.ChangedTo.Add(new ChangedTo
            {
                OldReservation = oldReso,
                NewReservation = newReso
            });
            db.SaveChanges();
        }

        //******EMAIL STATEMENTS*******************************************************

        /* Returns all 60 day reservations that have not paid, have not been canceled and that have 45 days or less left until the start date 
         * 
         */
        public static List<Reservations> GetReservationsForEmail()
        {
            using DatabaseContext db = new DatabaseContext();
            var toEmailList = db
                .Reservations
                .Include("Card")
                .Include("ReservationType")
                .Where(r => r.ReservationType.ReservationID == 2) //2 is for 60 day reservations
                .Where(r => r.Paid == false)
                .Where(r => r.IsCanceled == false)
                .Where(r => (r.StartDate - DateTime.Now).Days <= 45)
                .ToList();
            return toEmailList;
        }

        //******REPORT STATEMENTS******************************************************

        /* Returns a list of reservations that are expected to arrive today
         * 
         */
        public static List<Reservations> GetDailyArrivals()
        {
            using DatabaseContext db = new DatabaseContext();
            var dailyArrivals = db
                .Reservations
                .Include("Card")
                .Include("ReservationType")
                .Where(r => r.StartDate == DateTime.Today)
                .Where(r => r.IsCanceled == false)
                .OrderBy(r => r.FirstName)
                .ToList();

            var lateArrivals = db
                .Reservations
                .Include("Card")
                .Include("ReservationType")
                .Where(r => r.Confirmed == true)
                .Where(r => r.StartDate < DateTime.Today)
                .Where(r => r.EndDate >= DateTime.Today)
                .Where(r => r.CheckedIn == false)
                .Where(r => r.IsCanceled == false)
                .OrderBy(r => r.FirstName)
                .ToList();

            return dailyArrivals.Concat(lateArrivals).ToList();
        }

        /* Returns a list of reservations where they are checked in but not checked out (ordered by room num)
         * 
         */
        public static List<Reservations> GetTodaysOccupancies()
        {
            using DatabaseContext db = new DatabaseContext();
            var todaysOccupancies = db
                .Reservations
                .Include("Card")
                .Include("ReservationType")
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
                        .Include("Card")
                        .Include("ReservationType")
                        .Where(r => r.StartDate <= curDate)
                        .Where(r => r.EndDate >= curDate)
                        .Where(r => r.ReservationType.ReservationID == i)
                        .Count();
                    resoCount.Add(count);
                }
                occupancyInfo.Add(resoCount);
                curDate.AddDays(1);
            }

            return occupancyInfo;
        }

        /* This query gets 30 days of income from the current date
         * 
         */
        public static List<float> GetThirtyDayIncomeInfo()
        {
            List<float> incomeList = new List<float>(30);
            using DatabaseContext db = new DatabaseContext();

            DateTime curDate = DateTime.Now;
            for(int i = 0; i < 30; i++)// For thrity days
            {
                var income =
                    (
                        from br in db.BaseRates
                        join brr in db.BaseRatesReservations on br.BaseRateID equals brr.BaseRates.BaseRateID
                        join r in db.Reservations on brr.Reservations.ReservationID equals r.ReservationID
                        where r.StartDate <= curDate
                        where r.EndDate >= curDate
                        where r.IsCanceled == false
                        where br.EffectiveDate.Date == curDate.Date
                        select br.Rate
                    ).Sum(); //join base rates with reservations via the many to many table base rates reservations, get the daily rate from the reservations that are for that day

                incomeList.Add(income);
            }

            return incomeList;
        }

        /* Returns the amount of money lost each day over a period of 30 days from today
         * 
         */
        public static List<float> GetIncentiveReportInfo()
        {
            List<float> losses = new List<float>(30);

            using DatabaseContext db = new DatabaseContext();

            DateTime curDate = DateTime.Now;
            for(int i = 0; i < 30; i++)
            {
                var loss = (
                        from br in db.BaseRates
                        join brr in db.BaseRatesReservations on br.BaseRateID equals brr.BaseRates.BaseRateID
                        join r in db.Reservations on brr.Reservations.ReservationID equals r.ReservationID
                        where r.StartDate.Date <= curDate.Date
                        where r.EndDate.Date >= curDate.Date
                        where r.IsCanceled == false
                        where r.ReservationType.ReservationID == 4 //4 is incentive
                        where br.EffectiveDate.Date == curDate.Date
                        select br.Rate
                    ).Sum();

                loss *= (float)0.2; // 20% of base rate is equal to the amount lost due to incentive rate
                losses.Add(loss);
            }
            return losses;
        }

        /*
         * 
         */
        public static List<Reservations> GetAllResosToBeBilled(Payments payment)
        {
            using DatabaseContext db = new DatabaseContext();
            List<Reservations> ressos = new List<Reservations>();

            ressos.Add(payment.Reservation);

            int curResID = payment.Reservation.ReservationID;
            while (true)
            {
                var reso = (
                    from ct in db.ChangedTo
                    join r in db.Reservations on ct.OldReservation equals r
                    where ct.NewReservation.ReservationID == curResID
                    select r)
                    .Include("Card")
                    .Include("ReservationType")
                    .SingleOrDefault();
                    
                if(reso == null)
                {
                    break;
                }
                else
                {
                    ressos.Add(reso);
                    curResID = reso.ReservationID;
                }
            }
            return ressos;

        }
        //*******TEST STATEMENTS********************************************************
        //public static void AddReservationTest()
        //{
        //    using DatabaseContext db = new DatabaseContext();

        //    BaseRates br1 = new BaseRates()
        //    {
        //        BaseRateID = 1,
        //        Rate = 10,
        //        EffectiveDate = DateTime.Now.Date,
        //        DateSet = DateTime.Now,
        //    };
        //    BaseRates br2 = new BaseRates()
        //    {
        //        BaseRateID = 2,
        //        Rate = 10,
        //        EffectiveDate = DateTime.Now.AddDays(1).Date,
        //        DateSet = DateTime.Now,
        //    };

        //    db.BaseRates.Add(br1);
        //    db.BaseRates.Add(br2);

        //    List<BaseRates> baseRates = new List<BaseRates>() { br1, br2 };

        //    Reservations reso = new Reservations()
        //    {
        //        ReservationID = 1,
        //        LastName = "Bob",
        //        FirstName = "Billy",
        //        Email = "ThisIsFake@Fake.com",
        //        ReservationType = new ReservationTypes() { ReservationID = 1, PercentOfBase = (float).8 },
        //        Price = 20,
        //        RoomNum = 1,
        //        StartDate = DateTime.Now.Date,
        //        EndDate = DateTime.Now.AddDays(2).Date,
        //        IsCanceled = false,
        //        Paid = true,
        //        PaymentDate = DateTime.Now.Date,
        //        Confirmed = false,
        //        CheckedIn = true,
        //        CheckedOut = false,
        //        BaseRates = baseRates
        //    };

        //    db.Reservations.Add(reso);

        //    var rates = db
        //        .Reservations
        //        .Where(r => r.ReservationID == 1)
        //        .First();

        //    db.SaveChanges();

        //    for(int i = 0; i < rates.BaseRates.Count; i++)
        //    {
        //        Console.WriteLine("Rate " + i + ": " + rates.BaseRates.ToList()[i].Rate);
        //    }

        //}
    }
}
