using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Models.Entities
{
    [Table("Bookings")]
    public class Booking
    {
        [Column("Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("BookingReference")]
        [Display(Name = "Booking Reference")]
        public string BookingReference { get; set; }

        [Required]
        [Column("FlightId")]
        public int FlightId { get; set; }
        public Flight? Flight { get; set; }

        [Required]
        [Column("PassengerId")]
        public int PassengerId { get; set; }
        public Passenger? Passenger { get; set; }

        [Required]
        [Column("UserId")]
        [Display(Name = "User Id")]
        public string UserId { get; set; }  // FK to AspNetUsers

        [Required]
        [Column("BookingDate")]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Column("IsPaid")]
        [Display(Name = "Paid")]
        public bool IsPaid { get; set; }
    }
}
