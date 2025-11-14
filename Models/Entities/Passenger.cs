using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace Models.Entities
{
    [Table("Passengers")]
    public class Passenger
    {
        [Column("Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("FullName")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } 

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
