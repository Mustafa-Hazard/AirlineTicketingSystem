using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineTicketingSystem.Models.Entities
{
    [Table("Bookings")]
    public class Booking
    {
        [Column("Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Column("BookingReference")]
        [Display(Name = "Booking Reference")]
        public required string BookingReference { get; set; }

        [Required]
        [Column("FlightId")]
        public int FlightId { get; set; }
        public Flight? Flight { get; set; }

        [Required]
        [Column("PassengerId")]
        public int PassengerId { get; set; }
        public Passenger? Passenger { get; set; }

        [Required, StringLength(50)]
        [Column("UserId")]
        [Display(Name = "User Id")]
        public required string UserId { get; set; }  // FK to AspNetUsers

        [Required]
        [Column("BookingDate")]
        [Display(Name = "Booking Date")]
        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

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
