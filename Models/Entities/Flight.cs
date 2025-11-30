using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineTicketingSystem.Models.Entities
{
    [Table("Flights")]
    public class Flight
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(10)]
        [Display(Name = "Flight Number")]
        public string FlightNumber { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Origin { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Destination { get; set; } = string.Empty;

        [StringLength(10)]
        [Display(Name = "Origin Airport Code")]
        public string? FromAirport { get; set; }

        [StringLength(10)]
        [Display(Name = "Destination Airport Code")]
        public string? ToAirport { get; set; }

        [Required]
        [Display(Name = "Departure Time")]
        public DateTime DepartureTime { get; set; }

        [Required]
        [Display(Name = "Arrival Time")]
        public DateTime ArrivalTime { get; set; }

        [NotMapped]
        [Display(Name = "Flight Duration")]
        public TimeSpan Duration => ArrivalTime - DepartureTime;

        // Seat counts per class
        [Required, Range(0, 999)]
        public int EconomySeats { get; set; }

        [Required, Range(0, 999)]
        public int BusinessSeats { get; set; }

        [Required, Range(0, 999)]
        public int FirstClassSeats { get; set; }

        // Available seats per class
        public int AvailableEconomySeats { get; set; }
        public int AvailableBusinessSeats { get; set; }
        public int AvailableFirstClassSeats { get; set; }

        // Price per class
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal EconomyPrice { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal BusinessPrice { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal FirstClassPrice { get; set; }

        public bool IsDelayed { get; set; }

        public ICollection<Booking>? Bookings { get; set; }

    }
}
