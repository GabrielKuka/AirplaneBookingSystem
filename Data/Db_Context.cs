using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AirplaneBookingSystem.Models;

namespace AirplaneBookingSystem.Data
{
    public class Db_Context : IdentityDbContext
    {
        public Db_Context(DbContextOptions<Db_Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserFlights>().HasKey(sc => new { sc.UserId, sc.FlightId });
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<UserFlights> UserFlights { get; set; }
    }
}
