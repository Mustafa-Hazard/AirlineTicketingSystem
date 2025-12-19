using AirlineTicketingSystem.Models.Entities;

namespace AirlineTicketingSystem.Models.ViewModels
{
    public class CustomerDashboardVM
    {
        // Statistics (for the cards at the top)
        public int TotalBookings { get; set; }
        public int UpcomingTrips { get; set; }
        public int CompletedTrips { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal PendingPayments { get; set; }
        public int TotalPassengers { get; set; }

        // Booking Lists
        public List<Booking> UpcomingBookings { get; set; } = new();
        public List<Booking> RecentBookings { get; set; } = new();

        // Passengers
        public List<Passenger> MyPassengers { get; set; } = new();
    }
}