using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineTicketingSystem.Models.Entities
{
    [Table("Passengers")]
    public class Passenger
    {
        [Key]
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Column("First_Name")]
        [Display(Name = "First Name")]
        public string First_Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Column("Last_Name")]
        [Display(Name = "Surname")]
        public string Last_Name { get; set; } = string.Empty;

        public string FullName => $"{First_Name} {Last_Name}";

        [Required, StringLength(20)]
        [Column("PassportNumber")]
        [Display(Name = "Passport Number")]
        public string PassportNumber { get; set; } = string.Empty;

        [Required, Range(1, 120)]
        [Column("Age")]
        public int Age { get; set; }

        [Column("Nationality")]
        public string Nationality { get; set; }

        [Required, EmailAddress, StringLength(100)]
        [Column("ContactEmail")]
        [Display(Name = "Email")]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone, StringLength(20)]
        [Column("PhoneNumber")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}
