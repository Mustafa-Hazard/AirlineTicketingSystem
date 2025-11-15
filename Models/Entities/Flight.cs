using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;    

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
        [Column("FlightNumber")]
        public string FlightNumber { get; set; } 
        [Required]
        [Column("Origin")]
        public string Origin { get; set; } 

        [Required]
        [Column("Destination")]
        public string Destination { get; set; } 
        [Required]
        [Column("Departure")]
        [Display(Name = "Departure")]
        public DateTime Departure { get; set; }

        [Required]
        [Column("Arrival")]
        [Display(Name = "Arrival")]
        public DateTime Arrival { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column("TotalSeats")]
        [Display(Name = "Total Seats")]
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
