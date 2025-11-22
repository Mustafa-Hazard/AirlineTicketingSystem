using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineTicketingSystem.Models.Entities
{
    [Table("Flights")]
    public class Flight
    {
        [Column("Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        [Column("FlightNumber")]
        [Display(Name = "Flight Number")]
        public required string FlightNumber { get; set; }
        
        [Required]
        [StringLength(100)]
        [Column("Origin")]
        public required string Origin { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Destination")]
        public required string Destination { get; set; }
        
        [Required]
        [Column("DepartureTime")]
        [Display(Name = "DepartureTime Time")]
        public DateTime DepartureTime{ get; set; }

        [Required]
        [Column("ArrivalTime")]
        [Display(Name = "ArrivalTime Time")]
        public DateTime ArrivalTime { get; set; }

        //FromAirport 
        [Column]
        public string? FromAirport { get; set; }
        //ToAirport
        [Column]
        public string? ToAirport { get; set; }


        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column("TotalSeats")]
        [Display(Name = "Total Seats")]
        public int TotalSeats { get; set; }

        [Required]
        [Column("AvailableSeats")]
        [Display(Name = "Seats Available")]
        public int AvailableSeats { get; set; }

        [Column("IsDelayed")]
        [Display(Name = "Is Delayed")]
        public bool IsDelayed { get; set; }

        //Navigation property one to many with Booking
        public ICollection<Booking>? Bookings { get; set; }
    }
}
