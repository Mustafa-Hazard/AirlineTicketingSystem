using System.ComponentModel.DataAnnotations;

namespace AirlineTicketingSystem.Models.ViewModels
{
    public class PassengerCreateVM
    {
        [Required]
        public string First_Name { get; set; } = string.Empty;

        [Required]
        public string Last_Name { get; set; } = string.Empty;

        [Required]
        public string PassportNumber { get; set; } = string.Empty;

        [Range(1, 120)]
        public int Age { get; set; }

        public string Nationality { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
    }

}
