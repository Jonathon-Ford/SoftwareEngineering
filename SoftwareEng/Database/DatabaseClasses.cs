/* Author Jonathon Ford
* 
* This page contains all of the classes Entity Framework will use to model our database
* These classes are also used for data manipulation within the code
* 
*/
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Users
{
    [Key]
    public int UserID { get; set; }// Not null
    [Required, MaxLength(30)]
    public string Username { get; set; }// Not null
    [Required, MaxLength(30)]
    public string Password { get; set; }// Not null
    [Required]
    public string RoleName { get; set; }// Not null
}
public class Reservations
{
    [Key]
    public int ReservationID { get; set; }//Not null
    [Required, MaxLength(30)]
    public string LastName { get; set; }// Not null
    [Required, MaxLength(30)]
    public string FirstName { get; set; }// Not null
    [Required, MaxLength(100)]
    public string Email { get; set; }
    public CreditCards Card { get; set; }
    [Required]
    public ReservationTypes ReservationType { get; set; }// Not null
    [Required]
    public double Price { get; set; }// Not null
    public int RoomNum { get; set; }
    [Required]
    [Column(TypeName = "Date")]
    public DateTime StartDate { get; set; }// Not null
    [Required]
    [Column(TypeName="Date")]
    public DateTime EndDate { get; set; }// Not null
    [Required]
    public bool IsCanceled { get; set; }// Not null
    [Column(TypeName ="Date")]
    public DateTime? DateCanceled { get; set; }
    [Required]
    public bool Paid { get; set; }// Not null
    [Column(TypeName = "Date")]
    public DateTime? PaymentDate { get; set; }
    [Required]
    public bool Confirmed { get; set; }// Not null
    [Required]
    public bool CheckedIn { get; set; }// Not null
    [Required]
    public bool CheckedOut { get; set; }// Not null
    [Required]
    public ICollection<BaseRates> BaseRates { get; set; }// Not null
}
public class Payments
{
    [Key]
    public int PaymentID { get; set; }// Not null
    [Required]
    public Reservations Reservation { get; set; }// Not null
    [Required]
    [Column(TypeName = "Date")]
    public DateTime PaymentDate { get; set; }
    public string Description { get; set; }
    [Required]
    public float Price { get; set; }// Not null
    [Required]
    public CreditCards Card { get; set; }
}
public class ReservationTypes
{
    [Key]
    public int ReservationID { get; set; }// Not null
    [Required, MaxLength(100)]
    public string Description { get;  set; }// Not null
    [Required]
    public float PercentOfBase { get; set; }// Not null
}
public class CreditCards
{
    [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
    public long CardNum { get; set; }// Key also Not null
    [Required]
    public int CVVNum { get; set; }// Not null
    [Required]
    [Column(TypeName = "Date")]
    public DateTime ExpiryDate { get; set; }// Not null
}
public class BaseRates
{
    [Key]
    public int BaseRateID { get; set; }//Not null
    [Required]
    public float Rate { get; set; }// Not null
    [Required]
    [Column(TypeName = "Date")]
    public DateTime EffectiveDate { get; set; }// Not null
    [Required]
    [Column(TypeName = "Date")]
    public DateTime DateSet { get; set; }
    public ICollection<Reservations> Reservations { get; set; }
}
public class ChangedTo
{
    [Required]
    [Key]
    [Column(Order = 1)]
    public Reservations OldReservation { get; set; }
    [Required]
    [Key]
    [Column(Order = 2)]
    public Reservations NewReservation { get; set; }
}
[Microsoft.EntityFrameworkCore.Keyless]
public class BaseRatesReservations
{
    [Required]
    public Reservations Reservations { get; set; }
    [Required]
    public BaseRates BaseRates { get; set; }
}