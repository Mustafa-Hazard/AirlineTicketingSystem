using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineTicketingSystem.Models.Entities
{
    [Table("Airports")]
    public class Airport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("City")]
        [Required, StringLength(100)]
        public string City { get; set; }

        [Column("Airport_Name")]
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Column("Iata_Code")]

        [Required, StringLength(5)]
        [Display(Name = "IATA Code")]
        public string IataCode { get; set; } 

        [Column("Country")]

        [StringLength(100)]
        public string Country { get; set; } 

        [NotMapped]
        public string DisplayName => $"{City} - {Name} ({IataCode})";
    }
}
