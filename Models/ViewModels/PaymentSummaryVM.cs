using System.ComponentModel.DataAnnotations;
using AirlineTicketingSystem.Models.Entities;

namespace AirlineTicketingSystem.Models.ViewModels
{
    public class PaymentSummaryVM
    {
        public int BookingId { get; set; }

        [Display(Name = "Booking Reference")]
        public string BookingReference { get; set; } = string.Empty;

        [Display(Name = "Passenger Name")]
        public string PassengerName { get; set; } = string.Empty;

        [Display(Name = "Flight Number")]
        public string FlightNumber { get; set; } = string.Empty;

        public string Origin { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        [Display(Name = "Departure Date")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy hh:mm tt}")]
        public DateTime DepartureDate { get; set; }

        [Display(Name = "Seat Class")]
        public string SeatClass { get; set; } = string.Empty;

        [Display(Name = "Number of Seats")]
        public int SeatCount { get; set; }

        [Display(Name = "Passenger Type")]
        public string PassengerType { get; set; } = string.Empty;

        [Display(Name = "Total Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Payment Status")]
        public bool IsPaid { get; set; }

        [Display(Name = "Payment Date")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime PaymentDate { get; set; }
    }
}