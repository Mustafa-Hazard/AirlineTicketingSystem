using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineTicketingSystem.Models.Entities
{
    [Table("Bookings")]
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Column("BookingReference")]
        [Display(Name = "Booking Reference")]
        public string BookingReference { get; set; } = string.Empty;

        [Required]
        [Column("FlightId")]
        public int FlightId { get; set; }

        [ForeignKey("FlightId")]
        public Flight? Flight { get; set; }

        [Required]
        [Column("PassengerId")]
        public int PassengerId { get; set; }

        [ForeignKey("PassengerId")]
        public Passenger? Passenger { get; set; }

        [Required]
        [StringLength(50)]
        [Column("UserId")]
        [Display(Name = "User Id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("BookingDate")]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(1, 10)]
        [Column("SeatCount")]
        [Display(Name = "Seats")]
        public int SeatCount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Price")]
        [Range(0, 1000000, ErrorMessage = "Total price must be positive.")]
        public decimal TotalPrice { get; set; }

        [Column("IsPaid")]
        [Display(Name = "Paid")]
        public bool IsPaid { get; set; }

        [NotMapped]
        public string BookingInfo => $"{BookingReference} - {Passenger?.FullName} ({Flight?.FlightNumber})";
    }
}
