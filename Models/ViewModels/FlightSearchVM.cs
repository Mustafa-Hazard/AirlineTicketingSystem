using AirlineTicketingSystem.Models.Entities;

namespace AirlineTicketingSystem.ViewModels
{
    public class FlightSearchVM
    {
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public DateTime? DepartureDate { get; set; }

        public List<Flight>? Results { get; set; }
    }
}
