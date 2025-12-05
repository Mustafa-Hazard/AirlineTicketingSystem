using System;

namespace AirlineTicketingSystem.Models.ViewModels
{
    public class FlightReportRowVM
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }

        public int BookingCount { get; set; }
        public int SeatsBooked { get; set; }
        public decimal PaidRevenue { get; set; }
    }
}
