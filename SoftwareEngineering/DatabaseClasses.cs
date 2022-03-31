using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareEngineering.Models
{
    public class Users
    {
        public int userID { get; set; }// Not null
        public string username { get; set; }// Not null
        public string password { get; set; }// Not null
    }
    public class UserRoles
    {
        public Users userID { get; set; }// Not null
        public Roles roleID { get; set; }// Not null
    }
    public class Roles
    {
        public int roleID { get; set; }// Not null
        public string roleName { get; set; }// Not null
    }
    public class Reservations
    {
        public int reservationID { get; set; }//Not null
        public string LastName { get; set; }// Not null
        public string FirstName { get; set; }// Not null
        public string Email { get; set; }
        public CreditCards cardNum { get; set; }
        public ReservationTypes reservationType { get; set; }// Not null
        public int price { get; set; }// Not null
        public int roomNum { get; set; }
        public DateTime startDate { get; set; }// Not null
        public DateTime endDate { get; set; }// Not null
        public bool isCanceled { get; set; }// Not null
        public DateTime dateCanceled { get; set; }
        public bool paid { get; set; }// Not null
        public bool confirmed { get; set; }// Not null
        public bool checkedIn { get; set; }// Not null
        public bool checkedOut { get; set; }// Not null
    }
    public class Payments
    {
        public int paymentID { get; set; }// Not null
    }
    public class ReservationTypes
    {

    }
    public class CreditCards
    {

    }
    public class BaseRates
    {

    }
}
