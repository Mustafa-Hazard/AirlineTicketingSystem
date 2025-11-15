using Microsoft.EntityFrameworkCore;
using AirlineTicketingSystem.Models.Entities;

namespace AirlineTicketingSystem.Models
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<Passenger> Passengers { get; set; } 
        public DbSet<Booking> Bookings { get; set; } 

    }
}
