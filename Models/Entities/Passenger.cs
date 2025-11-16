using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AirlineTicketingSystem.Models.Entities
{
    [Table("Passengers")]
    public class Passenger
    {
        [Column("P_Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("First_Name")]
        [Display(Name = "First Name")]
        public string First_Name { get; set; } 
        
        [Column("Last_Name")]
        [Display(Name = "Surname")]
        public string Last_Name { get; set; } 


        [Required]
        [Column("PassportNumber")]
        [Display(Name = "Passport Number")]
        public string PassportNumber { get; set; } 

        [Required]
        [Column("Age")]
        public int Age { get; set; }

        [Required]
        [Column("ContactEmail")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string ContactEmail { get; set; } 
    }
}
