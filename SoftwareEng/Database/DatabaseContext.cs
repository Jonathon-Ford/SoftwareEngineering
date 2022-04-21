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
            string dataSource = @"134.228.35.6";
            dataSource = "localhost";
            string database = "Ophelia's Oasis";
            string connString = @"Data Source=" + dataSource + ";Initial Catalog=" + database + ";Integrated Security=False; Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;User=team14;Password=team14";
            connString = @"Data Source=" + dataSource + ";Initial Catalog=" + database + ";Integrated Security=True; Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            options.UseSqlServer(connString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChangedTo>()
                .HasKey("OldReservationReservationID", "NewReservationReservationID");
            modelBuilder.Entity<Reservations>()
                .Property("StartDate")
                .HasColumnType("date");
            modelBuilder.Entity<Reservations>()
                .Property("EndDate")
                .HasColumnType("date");
        }
    }
}
