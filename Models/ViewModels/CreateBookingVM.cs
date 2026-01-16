using AirlineTicketingSystem.Models.Entities;
using AirlineTicketingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

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

    public PassengerType PassengerType { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.Now;

    // ================= Passenger (FORM FIELDS) =================
    [Required]
    public PassengerCreateVM Passenger { get; set; } = new PassengerCreateVM();

    // ================= UI =================
    public IEnumerable<SelectListItem>? Flights { get; set; }
}
