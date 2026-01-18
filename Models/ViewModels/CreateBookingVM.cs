using AirlineTicketingSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AirlineTicketingSystem.Models.ViewModels
{
    public class CreateBookingVM
    {
        // ================= Booking =================
        [Required]
        public int FlightId { get; set; }

        [Required]
        public SeatClass SeatClass { get; set; }

        [Required]
        [Range(1, 10)]
        public int SeatCount { get; set; }

        [Required]
        public PassengerType PassengerType { get; set; }

        // ================= Passenger (FORM FIELDS) =================
        [Required]
        public PassengerCreateVM Passenger { get; set; } = new PassengerCreateVM();

        // Dropdown
        public List<SelectListItem> Flights { get; set; } = new List<SelectListItem>();
    }
}
