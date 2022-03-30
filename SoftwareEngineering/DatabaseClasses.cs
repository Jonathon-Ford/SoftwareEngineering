using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareEngineering.Models
{
    public class Users
    {
        int userID { get; set; }
        string username { get; set; }
        string password { get; set; }
        string roleName { get; set; }
    }
    public class Reservations
    {
        int reservationID { get; set; }
        string LastName { get; set; }
        string FirstName { get; set; }
        string Email { get; set; }
        int cardNum { get; set; }
        int reservationType { get; set; }
        int price { get; set; }
        int roomNum { get; set; }


    }
    public class Payments
    {

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
