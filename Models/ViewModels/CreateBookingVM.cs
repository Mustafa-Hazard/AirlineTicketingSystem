using AirlineTicketingSystem.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace AirlineTicketingSystem.Models.ViewModels
{
    public class CreateBookingVM
    {
        [Required]
        public int FlightId { get; set; }

        [Required]
        public int PassengerId { get; set; }

        [Required]
        public string SeatNumber { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        // For dropdown/display
        public List<Flight>? AvailableFlights { get; set; }
        public List<string>? AvailableSeats { get; set; }
    }
}
