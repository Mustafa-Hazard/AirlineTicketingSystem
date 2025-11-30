using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineTicketingSystem.Models.Entities
{
    public enum SeatClass
    {
        Economy = 0,
        Business = 1,
        FirstClass = 2
    }

    public enum PassengerType
    {
        Adult = 0,
        Child = 1,
        Infant = 2
    }

    [Table("Bookings")]
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [StringLength(50)]
        [Column("BookingReference")]
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

        [StringLength(50)]
        [Column("UserId")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("BookingDate")]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(1, 10)]
        [Column("SeatCount")]
        public int SeatCount { get; set; }

        [Required]
        [Column("SeatClass")]
        public SeatClass SeatClass { get; set; } = SeatClass.Economy;

        [Required]
        [Column("PassengerType")]
        public PassengerType PassengerType { get; set; } = PassengerType.Adult;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Column("IsPaid")]
        public bool IsPaid { get; set; }

        [NotMapped]
        public string BookingInfo => $"{BookingReference} - {Passenger?.FullName} ({Flight?.FlightNumber})";
    }
}
