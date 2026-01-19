using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;

namespace AirlineTicketingSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DatabaseContext context)
        {
            // Ensure the database is created (runs migrations)
            context.Database.EnsureCreated();

            // Check if any users exist to avoid duplicate seeding
            if (context.Users.Any())
            {
                return; // Database has already been seeded
            }

            // Seed a default Admin user
            // Note: We hash the password here just like in the AuthController
            var adminUser = new User
            {
                UserName = "AeroNexa ControlX",
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin"
            };

            context.Users.Add(adminUser);

            // You can also add more complex seed logic here that 
            // depends on runtime conditions, unlike ModelBuilder.HasData

            context.SaveChanges();
        }
    }
}