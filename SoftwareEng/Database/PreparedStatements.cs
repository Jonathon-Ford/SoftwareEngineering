﻿/* Author Jonathon Ford
 * 
 * Created: 4/8/2022
 * Finished: 4/29/2022
 * 
 * This Class contains all communication between the database and the code
 * You can add and remove data using only these functions to insure that faulty data does not get put in the system
 * 
 * This file is somewhat large, to navigate I reccomend ctrl + F what class you want to look for
 * EX.) to find hotel sytem fucntion ctrl + F (Hotel system statements)
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoftwareEng.DataModels;
using static SoftwareEng.ReservationHandler;

namespace SoftwareEng
{
    public class PreparedStatements
    {
        public static int errno = 0;

        //******HOTEL SYSTEM STATEMENTS***********************************************************

        /// <summary>
        /// Attempts to send back a user if the username and password match a record in the database, if not it returns an empty user class
        /// and sets the errno to 1
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Users</returns>
        public static Users ValidateUser(string username, string password)
        {
            using DatabaseContext db = new DatabaseContext();

            var user = db
                .Users
                .Where(u => EF.Functions.Collate(u.Username, "SQL_Latin1_General_CP1_CS_AS") == username)
                .Where(u => EF.Functions.Collate(u.Password, "SQL_Latin1_General_CP1_CS_AS") == password)
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

        /// <summary>
        /// Adds a new base rate, if there is already a base rate for that day it is ok, this will be used in the future
        /// </summary>
        /// <param name="baserate"></param>
        public static void AddBaseRate(BaseRates baserate)
        {
            using DatabaseContext db = new DatabaseContext();
            db.BaseRates.Add(baserate);
            db.SaveChanges();
        }

        /// <summary>
        /// This function updates the database so a reservation is checked in
        /// </summary>
        /// <param name="toCheckIn"></param>
        public static void MarkReservationAsCheckedIn(Reservations toCheckIn)
        {
            toCheckIn.CheckedIn = true;

            using DatabaseContext db = new DatabaseContext();
            db.Entry(toCheckIn).State = EntityState.Modified;
            db.Entry(toCheckIn.Card).State = EntityState.Unchanged;
            db.Entry(toCheckIn.ReservationType).State = EntityState.Unchanged;
            db.SaveChanges();
        }

        /// <summary>
        /// This function updates the database so a reservation is checked out
        /// </summary>
        /// <param name="toCheckOut"></param>
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

        /// <summary>
        /// This function finds a reservation with 3 levels of specificity
        ///  First it will look for reservations with the desired name, if they have more than one res it will try to specify the search
        ///  with the optional parameters of card num / email and start date
        /// </summary>
        /// <param name="FName"></param>
        /// <param name="LName"></param>
        /// <param name="lastFourOfCard"></param>
        /// <param name="email"></param>
        /// <param name="startDate"></param>
        /// <returns>List(Reservations)</returns>
        public static List<Reservations> FindReservation(string FName, string LName, int? lastFourOfCard = null, string? email = default, DateTime startDate = default(DateTime))
        {
            using DatabaseContext db = new DatabaseContext();
            var curReservations = db
                .Reservations
                .Include("Card")
                .Include("ReservationType")
                .Include(r => r.BaseRates)
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

        /// <summary>
        /// This function gets a list for all of the base rates for each day of a stay
        /// </summary>
        /// Modified by AS 4/28/22 to add default rate
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>List(BaseRates)</returns>
        public static List<BaseRates> GetBaseRates(DateTime startDate, DateTime endDate)
        {
            using DatabaseContext db = new DatabaseContext();
            List<BaseRates> rates = new List<BaseRates>();

            for (var day = startDate.Date; day < endDate.Date; day = day.AddDays(1))
            {
                try
                {
                    var curPrice = db
                        .BaseRates
                        .Where(br => br.EffectiveDate.Date == day.Date)
                        .OrderByDescending(br => br.DateSet)
                        .ToList();

                    if(curPrice.Count == 0)
                    {
                        var defaultPrice = db
                            .BaseRates
                            .OrderByDescending(br => br.EffectiveDate)
                            .ThenByDescending(br => br.DateSet)
                            .First();

                        rates.Add(new BaseRates()
                        {
                            DateSet = defaultPrice.DateSet,
                            EffectiveDate = day,
                            Rate = defaultPrice.Rate
                        });
                    }
                    else
                    {
                        rates.Add(curPrice.First());
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw new Exception("ERROR: Could not find base rate for " + day + "\n");
                }
            }
            return rates;
        }

        /// <summary>
        /// Gets all the details for the given reservation type
        /// </summary>
        /// Author: AS
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ReservationTypes GetReservationTypeDetails(ReservationTypes type)
        {
            try
            {
                using DatabaseContext db = new DatabaseContext();
                return db
                    .ReservationTypes
                    .Where(rt => rt.ReservationID == type.ReservationID)
                    .Single();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw new Exception("ERROR: Could not retrieve details for " + type.Description + " reservation type");
            }
        }

        /// <summary>
        /// Finds a credit card based on the card number
        /// </summary>
        /// Author: AS
        /// <param name="card"></param>
        /// <returns></returns>
        public static CreditCards FindCardByNum(CreditCards card)
        {
            using DatabaseContext db = new DatabaseContext();
            return db
                .CreditCards
                .Where(cc => cc.CardNum == card.CardNum)
                .Single();
        }

        /// <summary>
        /// This function updates a reservation record.
        /// Takes in the edited reservation (note you must have found the reservation with the correct key)
        /// *****WARNING : You should not change the date with this function, use change reservation date func*****
        /// </summary>
        /// <param name="editedReso"></param>
        public static void UpdateReservation(Reservations editedReso)
        {
            using DatabaseContext db = new DatabaseContext();
            db.Entry(editedReso).State = EntityState.Modified;
            db.SaveChanges();
        }

        /* 
         * Takes: the reservation to add
         */
        /// <summary>
        /// This function adds a reservation to the reservation table. 
        /// Takes: the reservation to add
        /// </summary>
        /// <param name="resoToAdd"></param>
        public static void AddReservation(Reservations resoToAdd)
        {
            using DatabaseContext db = new DatabaseContext();

            resoToAdd.ReservationType = db.ReservationTypes.Where(rt => rt.Description == resoToAdd.ReservationType.Description).First();
            db.Reservations.Add(resoToAdd);
            db.Entry(resoToAdd.Card).State = EntityState.Unchanged;
            db.Entry(resoToAdd.ReservationType).State = EntityState.Unchanged;

            foreach(var rate in resoToAdd.BaseRates)
                if(rate.BaseRateID != 0)
                    db.Entry(rate).State = EntityState.Unchanged;
            db.SaveChanges();
        }

        /// <summary>
        /// This function marks the reservation given as canceled and sets the date it was canceled to the current day
        /// </summary>
        /// <param name="toCancel"></param>
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

        /// <summary>
        /// This function gives you the availability for the day
        /// </summary>
        /// <param name="day"></param>
        /// <returns>int</returns>
        public static int GetAvailability(DateTime day)
        {
            using DatabaseContext db = new DatabaseContext();
            var count = db
                .Reservations
                .Where(r => r.StartDate <= day)
                .Where(r => r.EndDate >= day)
                .Where(r => r.IsCanceled == false)
                .Count();

            return TOTAL_ROOMS - count;
        }

        /// <summary>
        /// Marks a reservation as being changed to the new reso. 
        /// Takes: The old reservation (this will ensure it is canceled) and
        /// a new reservation
        /// </summary>
        /// <param name="oldReso"></param>
        /// <param name="newReso"></param>
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

        /// <summary>
        /// Returns all reservations that have not been cancelled, checked in, or confirmed and were supposed to start the previous day
        /// </summary>
        /// Author: AS
        public static List<Reservations> GetNoShowReservations()
        {
            using DatabaseContext db = new DatabaseContext();

            return db.Reservations
                .Where(r => !r.IsCanceled && !r.Confirmed && !r.CheckedIn)
                .Where(r => r.StartDate == DateTime.Now.Date.AddDays(-1))
                .ToList();
        }

        //******EMAIL STATEMENTS*******************************************************

        /// <summary>
        /// Returns all 60 day reservations that have not paid, have not been canceled and that have 45 days or less left until the start date 
        /// </summary>
        /// <returns>List(Reserations)</returns>
        public static List<Reservations> GetReservationsForEmail()
        {
            var fortyFiveDaysOut = DateTime.Now.AddDays(45).Date;
            var thirtyDaysOut = DateTime.Now.AddDays(30).Date;
            using DatabaseContext db = new DatabaseContext();
            return db
                .Reservations
                .Include(r  => r.Card)
                .Include(r => r.ReservationType)
                .Include(r => r.BaseRates)
                .Where(r => r.ReservationType.ReservationID == (int)ReservationTypeCode.SixtyDay)
                .Where(r => r.Paid == false)
                .Where(r => r.IsCanceled == false)
                .Where(r => r.StartDate <= fortyFiveDaysOut)
                .Where(r => r.StartDate >= thirtyDaysOut)
                .ToList();
        }

        /// <summary>
        /// Returns all 60 day reservations that have not paid, have not been cancelled, and start in less than 30 days
        /// </summary>
        /// Author: AS
        /// <returns>List(Reservations)</returns>
        public static List<Reservations> GetReservationsToCancelForEmail()
        {
            var thirtyDaysOut = DateTime.Now.AddDays(30).Date;
            using DatabaseContext db = new DatabaseContext();
            return db
                .Reservations
                .Include(r => r.Card)
                .Include(r => r.ReservationType)
                .Include(r => r.BaseRates)
                .Where(r => r.ReservationType.ReservationID == (int)ReservationTypeCode.SixtyDay)
                .Where(r => r.Paid == false)
                .Where(r => r.IsCanceled == false)
                .Where(r => r.StartDate < thirtyDaysOut)
                .ToList();
        }

        //******REPORT STATEMENTS******************************************************

        /*
         * 
         */
        public static List<Reservations> GetTodaysGuests()
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

            List<Reservations> result = dailyArrivals;
            return result;
        }

        /// <summary>
        /// Returns a list of reservations that are expected to arrive today
        /// </summary>
        /// <returns>List(Reservations)</returns>
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

        /// <summary>
        /// Returns a list of reservations where they are checked in but not checked out (ordered by room num)
        /// </summary>
        /// <returns>List(Reservations)</returns>
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

        /// <summary>
        /// returns a list of a list of a list of reservations in the form:
        /// object[day][reservation_type - 1] = count
        /// </summary>
        /// <returns>List(List(int))</returns>
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
                        .Where(r => r.ReservationType.ReservationID == j)
                        .Where(r => r.IsCanceled == false)
                        .Count();
                    resoCount.Add(count);
                }
                occupancyInfo.Add(resoCount);
                curDate = curDate.AddDays(1);
            }

            return occupancyInfo;
        }

        /// <summary>
        /// This query gets 30 days of income from the current date
        /// </summary>
        /// <returns>List(float)</returns>
        public static List<float> GetThirtyDayIncomeInfo()
        {
            List<float> incomeList = new List<float>(30);
            using DatabaseContext db = new DatabaseContext();

            DateTime curDate = DateTime.Now.Date;
            for(int i = 0; i < 30; i++)// For thrity days
            {
                var resos = db
                    .Reservations
                    .Where(r => r.StartDate <= curDate)
                    .Where(r => r.EndDate >= curDate)
                    .Where(r => r.IsCanceled == false)
                    .Include("BaseRates")
                    .ToList(); //join base rates with reservations via the many to many table base rates reservations, get the daily rate from the reservations that are for that day

                float perDayTotal = 0;
                //I am so sorry to any man, beast, celestial or otherwise that has to look upon my madness
                for(int j = 0; j < resos.Count; j++)//For how many resos we got...
                {
                    for(int k = 0; k < resos[j].BaseRates.Count; k++)//For how many base rates are in the reso we are looking at...
                    {
                        if (resos[j].BaseRates.ToList()[k].EffectiveDate == curDate)
                        {//If this is the day we are looking for....
                            perDayTotal += resos[j].BaseRates.ToList()[k].Rate; //Add the rate to the others
                            break;
                        }
                    }
                }

                incomeList.Add(perDayTotal);
                curDate = curDate.AddDays(1);
            }

            return incomeList;
        }

        /// <summary>
        /// Returns the amount of money lost each day over a period of 30 days from today
        /// </summary>
        /// <returns>List(float)</returns>
        public static List<float> GetIncentiveReportInfo()
        {
            List<float> losses = new List<float>(30);

            using DatabaseContext db = new DatabaseContext();

            DateTime curDate = DateTime.Now.Date;
            for(int i = 0; i < 30; i++)
            {
                var resos = db
                    .Reservations
                    .Where(r => r.StartDate <= curDate)
                    .Where(r => r.EndDate >= curDate)
                    .Where(r => r.IsCanceled == false)
                    .Where(r => r.ReservationType.ReservationID == (int)ReservationHandler.ReservationTypeCode.Incentive)
                    .Include("BaseRates")
                    .ToList(); //join base rates with reservations via the many to many table base rates reservations, get the daily rate from the reservations that are for that day

                float perDayLoss = 0;
                //I am so sorry to any man, beast, celestial or otherwise that has to look upon my madness
                for (int j = 0; j < resos.Count; j++)//For how many resos we got...
                {
                    for (int k = 0; k < resos[j].BaseRates.Count; k++)//For how many base rates are in the reso we are looking at...
                    {
                        if (resos[j].BaseRates.ToList()[k].EffectiveDate == curDate)
                        {//If this is the day we are looking for....
                            perDayLoss += resos[j].BaseRates.ToList()[k].Rate; //Add the rate to the others
                            break;
                        }
                    }
                }

                perDayLoss *= (float)0.2;
                losses.Add(perDayLoss);
                curDate = curDate.AddDays(1);
            }
            return losses;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns>List(Reservations)</returns>
        public static List<Reservations> GetAllResosToBeBilled(Reservations reservation)
        {
            using DatabaseContext db = new DatabaseContext();
            List<Reservations> ressos = new List<Reservations>();

            ressos.Add(reservation);

            int curResID = reservation.ReservationID;
            while (true)
            {
                var reso = db
                    .ChangedTo
                    .Include(ct => ct.OldReservation)
                    .Include(ct => ct.OldReservation.BaseRates)
                    .Include(ct => ct.OldReservation.Card)
                    .Include(ct => ct.OldReservation.ReservationType)
                    .Where(ct => ct.NewReservation.ReservationID == curResID)
                    .Select(ct => ct.OldReservation).SingleOrDefault();//find an old reservation that is linked to the current reservation

                    
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

        /// <summary>
        /// Records a payment
        /// </summary>
        /// Author: AS
        /// <param name="payment"></param>
        public static void AddPayment(Payments payment)
        {
            using DatabaseContext db = new DatabaseContext();

            payment.Reservation.BaseRates.Clear();
            db.Payments.Add(payment);
            db.Entry(payment.Card).State = EntityState.Unchanged;
            db.Entry(payment.Reservation).State = EntityState.Unchanged;
            db.Entry(payment.Reservation.ReservationType).State = EntityState.Unchanged;
            db.Entry(payment.Reservation.Card).State = EntityState.Unchanged;
            

            db.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardNum"></param>
        /// <param name="cvv"></param>
        /// <param name="expireDate"></param>
        /// <returns>CreditCards</returns>
        /// 
        public static CreditCards AddCardInfo(long cardNum, int cvv, DateTime expireDate)
        {
            using DatabaseContext db = new DatabaseContext();
            

            var card = db
                .CreditCards
                .Where(u => u.CardNum == cardNum)
                .SingleOrDefault();

            if (card == null)
            {
                CreditCards newCard = new CreditCards { CardNum = cardNum, CVVNum = cvv, ExpiryDate = expireDate };
                db.CreditCards.Add(newCard);
                db.SaveChanges();
                return newCard; //Return an empty user class to show it was not found
            } else
            {
                card.CardNum = cardNum;
                card.CVVNum = cvv;
                card.ExpiryDate = expireDate;
                db.SaveChanges();
                return card;
            }

        }

        //*******TEST STATEMENTS********************************************************
        public static void PopulateWithTestData(int numToAdd)
        {
            DateTime curDate = DateTime.Now.Date;
            Random r = new Random();
            for(int i = 0; i < 60; i++)
            {
                BaseRates toAdd = new BaseRates();
                float rate = r.Next(50, 300);
                toAdd.Rate = rate;
                toAdd.DateSet = curDate;
                toAdd.EffectiveDate = curDate.AddDays(i);
                AddBaseRate(toAdd);
            }

            int numRes = r.Next(0, ReservationHandler.TOTAL_ROOMS);

            try
            {
                CreditCards dontNeed = AddCardInfo( 1111111111111111, 123, DateTime.Now.AddYears(2));
            }
            catch(Exception e) { }

            for(int i = 0; i < numToAdd; i++)
            {
                int randDaysAway = r.Next(0, 40);
                int randStayLen = r.Next(1, 10);

                Reservations toAdd = new Reservations();
                toAdd.StartDate = DateTime.Now.AddDays(randDaysAway);
                toAdd.EndDate = DateTime.Now.AddDays(randDaysAway + randStayLen);

                bool keepGoing = true;
                for(int j = randDaysAway; j < randDaysAway + randStayLen; j++)
                {
                    int count = GetAvailability(DateTime.Now.AddDays(j));
                    if(count == 0)
                    {
                        keepGoing = false;
                        break;
                    }
                }
                if (!keepGoing)
                {
                    continue;
                }

                toAdd.FirstName = "Test";
                toAdd.LastName = "McTestface" + i;

                int type = r.Next(1, 4);
                switch (type)
                {
                    case 1: toAdd.ReservationType = new ReservationTypes() { Description = "Prepaid", ReservationID = 1};break;
                    case 2: toAdd.ReservationType = new ReservationTypes() { Description = "SixtyDay", ReservationID = 2 }; break;
                    case 3: toAdd.ReservationType = new ReservationTypes() { Description = "Conventional", ReservationID = 3 }; break;
                    case 4: toAdd.ReservationType = new ReservationTypes() { Description = "Incentive", ReservationID = 4 }; break;
                }

                toAdd.Email = "fake" + i + "@notreal.com";
                toAdd.Card = new CreditCards () { CardNum = 1111111111111111, CVVNum = 123, ExpiryDate = DateTime.Now.AddYears(2) };

                toAdd.BaseRates = GetBaseRates(toAdd.StartDate, toAdd.EndDate);

                float sum = 0;
                for(int j = 0; j < toAdd.BaseRates.Count; j++)
                {
                    sum += toAdd.BaseRates.ToList()[j].Rate;
                }

                toAdd.Price = sum;

                AddReservation(toAdd);
            }
        }
    }
}
