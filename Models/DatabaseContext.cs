using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Models
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
