using AirlineTicketingSystem.Models.Entities;

namespace AirlineTicketingSystem.Models.ViewModels
{
    public class AdminDashboardVM
    {
        // Today's Statistics
        public int TodayBookings { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TodayFlights { get; set; }

        // Main Statistics Cards
        public int TotalFlights { get; set; }
        public int TotalBookings { get; set; }
        public int PaidBookings { get; set; }
        public int UnpaidBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal PotentialRevenue { get; set; }
        public int TotalPassengers { get; set; }
        public int TotalUsers { get; set; }

        // Popular Routes (for bar chart table)
        public List<PopularRouteVM> PopularRoutes { get; set; } = new();

        // Seat Class Distribution (for pie chart table)
        public List<SeatClassStatVM> SeatClassStats { get; set; } = new();

        // Recent Bookings (for table)
        public List<Booking> RecentBookings { get; set; } = new();

        // Upcoming Flights (for table)
        public List<Flight> UpcomingFlights { get; set; } = new();
    }

    // Nested class for Popular Routes
    public class PopularRouteVM
    {
        public string Route { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
    }

    // Nested class for Seat Class Statistics
    public class SeatClassStatVM
    {
        public string SeatClass { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }
}