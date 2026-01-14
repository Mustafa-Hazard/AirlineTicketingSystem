namespace AirlineTicketingSystem.Services
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(string toEmail, string passengerName,
            string flightNumber, string destination, DateTime departureDate,
            string bookingReference, decimal totalAmount);
    }
}