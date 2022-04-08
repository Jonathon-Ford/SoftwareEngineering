using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SoftwareEng.DataModels
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Reservations> Reservations { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<ReservationTypes> ReservationTypes { get; set; }
        public DbSet<CreditCards> CreditCards { get; set; }
        public DbSet<BaseRates> BaseRates { get; set; }
        public DbSet<ChangedTo> ChangedTo { get; set; }
        public DbSet<BaseRatesReservations> BaseRatesReservations{ get; set; }
        public string DbPath { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //Use Bao's hosted server probably but for testing "this works on my machine" (tho it is default and so long as you werent fancy with the install this should work for you too)
            string dataSource = @"134.228.35.6";
            string database = "Ophelia's Oasis";
            string connString = @"Data Source=" + dataSource + ";Initial Catalog=" + database + ";Integrated Security=False; Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;User=team14;Password=team14";
            options.UseSqlServer(connString);
        }
    }
}
