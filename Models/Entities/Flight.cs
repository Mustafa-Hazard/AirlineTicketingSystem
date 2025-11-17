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
        [Column("Departure")]
        [Display(Name = "Departure Time")]
        public DateTime Departure { get; set; }

        [Required]
        [Column("Arrival")]
        [Display(Name = "Arrival Time")]
        public DateTime Arrival { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column("TotalSeats")]
        [Display(Name = "Total Sea  ts")]
        public int TotalSeats { get; set; }

        [Required]
        [Column("SeatsAvailable")]
        [Display(Name = "Seats Available")]
        public int SeatsAvailable { get; set; }

        [Column("IsDelayed")]
        [Display(Name = "Is Delayed")]
        public bool IsDelayed { get; set; }
    }
}
