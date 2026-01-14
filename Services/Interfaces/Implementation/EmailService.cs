using AirlineTicketingSystem.Models.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AirlineTicketingSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendBookingConfirmationAsync(string toEmail, string passengerName,
            string flightNumber, string destination, DateTime departureDate,
            string bookingReference, decimal totalAmount)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = $"✈️ Flight Booking Confirmation - {bookingReference}";

                var builder = new BodyBuilder
                {
                    HtmlBody = GetBookingConfirmationHtml(passengerName, flightNumber, destination,
                        departureDate, bookingReference, totalAmount)
                };

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Booking confirmation email sent to {toEmail} for booking {bookingReference}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send booking confirmation email to {toEmail}");
                throw new Exception($"Failed to send confirmation email: {ex.Message}");
            }
        }

        private string GetBookingConfirmationHtml(string passengerName, string flightNumber,
            string destination, DateTime departureDate, string bookingReference, decimal totalAmount)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Booking Confirmation</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 650px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 0 20px rgba(0,0,0,0.1);
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px 20px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
            font-weight: 600;
        }}
        .header p {{
            margin: 10px 0 0 0;
            font-size: 16px;
            opacity: 0.9;
        }}
        .content {{
            padding: 30px 20px;
        }}
        .greeting {{
            font-size: 18px;
            margin-bottom: 20px;
            color: #333;
        }}
        .booking-ref {{
            background-color: #f8f9fa;
            border-left: 4px solid #667eea;
            padding: 15px;
            margin: 20px 0;
            font-size: 16px;
        }}
        .booking-ref strong {{
            color: #667eea;
            font-size: 20px;
        }}
        .details-section {{
            background-color: #f8f9fa;
            border-radius: 8px;
            padding: 20px;
            margin: 20px 0;
        }}
        .details-section h2 {{
            margin-top: 0;
            color: #667eea;
            font-size: 20px;
            border-bottom: 2px solid #667eea;
            padding-bottom: 10px;
        }}
        .detail-row {{
            display: flex;
            justify-content: space-between;
            padding: 12px 0;
            border-bottom: 1px solid #e0e0e0;
        }}
        .detail-row:last-child {{
            border-bottom: none;
        }}
        .detail-label {{
            font-weight: 600;
            color: #555;
        }}
        .detail-value {{
            color: #333;
            text-align: right;
        }}
        .flight-route {{
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 20px 0;
            font-size: 18px;
            font-weight: bold;
        }}
        .flight-route .city {{
            color: #667eea;
        }}
        .flight-route .arrow {{
            margin: 0 15px;
            color: #999;
        }}
        .price-section {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            border-radius: 8px;
            margin: 20px 0;
        }}
        .price-section .label {{
            font-size: 14px;
            opacity: 0.9;
        }}
        .price-section .amount {{
            font-size: 32px;
            font-weight: bold;
            margin: 10px 0;
        }}
        .important-notice {{
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            margin: 20px 0;
            border-radius: 4px;
        }}
        .important-notice h3 {{
            margin-top: 0;
            color: #856404;
            font-size: 16px;
        }}
        .important-notice ul {{
            margin: 10px 0;
            padding-left: 20px;
        }}
        .important-notice li {{
            color: #856404;
            margin: 5px 0;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #666;
            font-size: 14px;
        }}
        .footer p {{
            margin: 5px 0;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>✈️ Booking Confirmed!</h1>
            <p>Your flight has been successfully booked</p>
        </div>
        
        <div class='content'>
            <p class='greeting'>Dear <strong>{passengerName}</strong>,</p>
            
            <p>Thank you for choosing our Airline Ticketing System! We're excited to have you on board.</p>
            
            <div class='booking-ref'>
                <strong>Booking Reference: {bookingReference}</strong>
            </div>
            
            <div class='flight-route'>
                <span class='city'>Departure</span>
                <span class='arrow'>✈️ →</span>
                <span class='city'>{destination}</span>
            </div>
            
            <div class='details-section'>
                <h2>Flight Details</h2>
                <div class='detail-row'>
                    <span class='detail-label'>Flight Number:</span>
                    <span class='detail-value'>{flightNumber}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Destination:</span>
                    <span class='detail-value'>{destination}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Departure Date:</span>
                    <span class='detail-value'>{departureDate:dddd, MMMM dd, yyyy}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Departure Time:</span>
                    <span class='detail-value'>{departureDate:hh:mm tt}</span>
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Passenger:</span>
                    <span class='detail-value'>{passengerName}</span>
                </div>
            </div>
            
            <div class='price-section'>
                <div class='label'>Total Amount</div>
                <div class='amount'>${totalAmount:N2}</div>
            </div>
            
            <div class='important-notice'>
                <h3>⚠️ Important Information</h3>
                <ul>
                    <li>Please arrive at the airport <strong>at least 2 hours</strong> before departure for domestic flights and <strong>3 hours</strong> for international flights.</li>
                    <li>Bring a valid government-issued ID and your booking reference.</li>
                    <li>Check-in opens 3 hours before departure and closes 45 minutes before departure.</li>
                    <li>Baggage allowance varies by ticket class. Please check your ticket details.</li>
                </ul>
            </div>
            
            <p style='margin-top: 30px;'>Need to make changes to your booking? Log in to your account or contact our support team.</p>
            
            <p style='color: #666; font-size: 14px; margin-top: 20px;'>
                Keep this email for your records. You may be asked to show it at check-in.
            </p>
        </div>
        
        <div class='footer'>
            <p><strong>Airline Ticketing System</strong></p>
            <p>© {DateTime.Now.Year} All rights reserved.</p>
            <p style='margin-top: 15px; font-size: 12px;'>
                This is an automated email. Please do not reply to this message.
            </p>
            <p style='margin-top: 10px; font-size: 12px;'>
                If you have any questions, please contact our support team.
            </p>
        </div>
    </div>
</body>
</html>";
        }
    }
}