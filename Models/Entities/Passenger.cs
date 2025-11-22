using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AirlineTicketingSystem.Models.Entities
{
    [Table("Passengers")]
    public class Passenger
    {
        [Column("Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("First_Name")]
        [Display(Name = "First Name")]
        public required string First_Name { get; set; } 
        
        [Column("Last_Name")]
        [Display(Name = "Surname")]
        public required string Last_Name { get; set; }

        public string FullName => $"{First_Name} {Last_Name}";

        [Required]
        [Column("PassportNumber")]
        [Display(Name = "Passport Number")]
        public required string PassportNumber { get; set; } 

        [Required]
        [Column("Age")]
        public int Age { get; set; }

        [Required]
        [Column("ContactEmail")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public required string ContactEmail { get; set; }

        //Navigation property one to many with Booking
        public ICollection<Booking>? Bookings { get; set; }
    }
}
