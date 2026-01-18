namespace AirlineTicketingSystem.Services
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(string toEmail, string passengerName,
            string flightNumber, string destination, DateTime departureDate,
            string bookingReference, decimal totalAmount);

        Task SendEmailAsync(string toEmail, string subject, string htmlBody);

        Task SendPaymentSuccessEmailAsync(
            string toEmail,
            string passengerName,
            string flightNumber,
            string origin,
            string destination,
            DateTime departureDate,
            string bookingReference,
            decimal totalAmount
        );

        // ✅ NEW: Payment success email + PDF attachment
        Task SendPaymentSuccessEmailWithPdfAsync(
            string toEmail,
            string passengerName,
            string flightNumber,
            string origin,
            string destination,
            DateTime departureDate,
            string bookingReference,
            decimal totalAmount,
            byte[] pdfBytes,
            string pdfFileName
        );
    

Task SendFlightDelayNotificationAsync(
            string toEmail,
            string passengerName,
            string flightNumber,
            string origin,
            string destination,
            DateTime departureTime
        );
    }
    
}
