using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineTicketingSystem.Models.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Column("hashed_password")]
        public string HashedPassword { get; set; } = string.Empty;

        [Column("role")]
        public string? Role { get; set; }   // "Admin" or "Customer"
    }
}
