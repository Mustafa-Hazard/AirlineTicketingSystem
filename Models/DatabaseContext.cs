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
        public DbSet<User> Users { get; set; }

        public DbSet<Airport> Airports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 👇 simple seeding – you can add more airports here
            modelBuilder.Entity<Airport>().HasData(
            new Airport { Id = 1, City = "Karachi", Name = "Jinnah Intl", IataCode = "KHI", Country = "Pakistan" },
            new Airport { Id = 2, City = "Lahore", Name = "Allama Iqbal Intl", IataCode = "LHE", Country = "Pakistan" },
            new Airport { Id = 3, City = "Islamabad", Name = "Islamabad Intl", IataCode = "ISB", Country = "Pakistan" },
            new Airport { Id = 4, City = "Dubai", Name = "Dubai Intl", IataCode = "DXB", Country = "UAE" },
            new Airport { Id = 5, City = "Abu Dhabi", Name = "Zayed Intl", IataCode = "AUH", Country = "UAE" },
            new Airport { Id = 6, City = "Jeddah", Name = "King Abdulaziz", IataCode = "JED", Country = "Saudi Arabia" },
            new Airport { Id = 7, City = "Riyadh", Name = "King Khalid", IataCode = "RUH", Country = "Saudi Arabia" },
            new Airport { Id = 8, City = "London", Name = "Heathrow", IataCode = "LHR", Country = "UK" },
            new Airport { Id = 9, City = "New York", Name = "JFK Intl", IataCode = "JFK", Country = "USA" },
            new Airport { Id = 10, City = "Doha", Name = "Hamad Intl", IataCode = "DOH", Country = "Qatar" }
            );
        }


    }
}
