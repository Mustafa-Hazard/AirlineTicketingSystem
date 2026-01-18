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

        // =========================================
        // ✅ 1) BOOKING CONFIRMATION EMAIL
        // =========================================
        public async Task SendBookingConfirmationAsync(
            string toEmail,
            string passengerName,
            string flightNumber,
            string destination,
            DateTime departureDate,
            string bookingReference,
            decimal totalAmount)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = $"✈️ Flight Booking Confirmation - {bookingReference}";

                var builder = new BodyBuilder
                {
                    HtmlBody = GetBookingConfirmationHtml(
                        passengerName,
                        flightNumber,
                        destination,
                        departureDate,
                        bookingReference,
                        totalAmount)
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

        // =========================================
        // ✅ 2) CUSTOM EMAIL (ANY SUBJECT/BODY)
        // =========================================
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = htmlBody };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Custom email sent to {toEmail} with subject '{subject}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send custom email to {toEmail}");
                throw new Exception($"Failed to send email: {ex.Message}");
            }
        }

        // =========================================
        // ✅ 3) PAYMENT SUCCESS EMAIL (NO PDF)
        // =========================================
        public async Task SendPaymentSuccessEmailAsync(
            string toEmail,
            string passengerName,
            string flightNumber,
            string origin,
            string destination,
            DateTime departureDate,
            string bookingReference,
            decimal totalAmount)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = $"✅ Payment Successful - {bookingReference}";

                var builder = new BodyBuilder
                {
                    HtmlBody = GetPaymentSuccessHtml(
                        passengerName,
                        flightNumber,
                        origin,
                        destination,
                        departureDate,
                        bookingReference,
                        totalAmount)
                };

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Payment success email sent to {toEmail} for booking {bookingReference}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send payment success email to {toEmail}");
                throw new Exception($"Failed to send payment success email: {ex.Message}");
            }
        }

        // =========================================
        // ✅ 4) PAYMENT SUCCESS EMAIL + PDF ATTACHMENT
        // =========================================
        public async Task SendPaymentSuccessEmailWithPdfAsync(
            string toEmail,
            string passengerName,
            string flightNumber,
            string origin,
            string destination,
            DateTime departureDate,
            string bookingReference,
            decimal totalAmount,
            byte[] pdfBytes,
            string pdfFileName)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = $"✅ Payment Successful - {bookingReference}";

                var builder = new BodyBuilder
                {
                    HtmlBody = GetPaymentSuccessHtml(
                        passengerName,
                        flightNumber,
                        origin,
                        destination,
                        departureDate,
                        bookingReference,
                        totalAmount)
                };

                // ✅ Attach PDF
                if (pdfBytes != null && pdfBytes.Length > 0)
                {
                    builder.Attachments.Add(pdfFileName, pdfBytes, ContentType.Parse("application/pdf"));
                }

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Payment success email (with PDF) sent to {toEmail} for booking {bookingReference}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send payment success email (with PDF) to {toEmail}");
                throw new Exception($"Failed to send payment success email with PDF: {ex.Message}");
            }
        }

        // =========================================
        // ✅ 5) BOOKING CANCELLED EMAIL
        // =========================================
        public async Task SendBookingCancelledEmailAsync(
            string toEmail,
            string passengerName,
            string flightNumber,
            string origin,
            string destination,
            DateTime departureDate,
            string bookingReference)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = $"❌ Booking Cancelled - {bookingReference}";

                var builder = new BodyBuilder
                {
                    HtmlBody = GetBookingCancelledHtml(
                        passengerName,
                        flightNumber,
                        origin,
                        destination,
                        departureDate,
                        bookingReference)
                };

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Booking cancelled email sent to {toEmail} for booking {bookingReference}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send booking cancelled email to {toEmail}");
                throw new Exception($"Failed to send booking cancelled email: {ex.Message}");
            }
        }
        // Send flight delay notification
        public async Task SendFlightDelayNotificationAsync(
            string toEmail,
            string passengerName,
            string flightNumber,
            string origin,
            string destination,
            DateTime departureTime)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = $"⚠️ Flight {flightNumber} Delay Notification";

                var builder = new BodyBuilder
                {
                    HtmlBody = $@"
                <h2>Dear {passengerName},</h2>
                <p>Your flight <strong>{flightNumber}</strong> from <strong>{origin}</strong> to <strong>{destination}</strong> has been delayed.</p>
                <p>New Departure Time: {departureTime:dddd, MMMM dd, yyyy, hh:mm tt}</p>
                <p>We apologize for the inconvenience and recommend checking the flight status for any further updates.</p>
                <p>Thank you for your understanding.</p>
                <p>Best regards,<br/>AeroNexa Airlines</p>"
                };

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Flight delay notification sent to {toEmail} for flight {flightNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send flight delay notification to {toEmail}");
                throw new Exception($"Failed to send flight delay notification: {ex.Message}");
            }
        }
        // ============================================================
        // ===================== HTML TEMPLATES ========================
        // ============================================================

        // ✅ BOOKING CONFIRMATION HTML (YOUR SAME STYLE)
        private string GetBookingConfirmationHtml(
            string passengerName,
            string flightNumber,
            string destination,
            DateTime departureDate,
            string bookingReference,
            decimal totalAmount)
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
            <p>Thank you for choosing AeroNexa! We're excited to have you on board.</p>

            <div class='booking-ref'>
                <strong>Booking Reference: {bookingReference}</strong>
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
                    <li>Please arrive at the airport <strong>at least 2 hours</strong> before departure.</li>
                    <li>Bring a valid government-issued ID and your booking reference.</li>
                    <li>Check-in closes 45 minutes before departure.</li>
                </ul>
            </div>

            <p style='color: #666; font-size: 13px; margin-top: 15px;'>
                This is an automated email. Please do not reply.
            </p>
        </div>

        <div class='footer'>
            <p><strong>AeroNexa</strong></p>
            <p>© {DateTime.Now.Year} All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        // ✅ PAYMENT SUCCESS HTML (GREEN THEME)
        private string GetPaymentSuccessHtml(
            string passengerName,
            string flightNumber,
            string origin,
            string destination,
            DateTime departureDate,
            string bookingReference,
            decimal totalAmount)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Payment Successful</title>
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
            background: linear-gradient(135deg, #2ecc71 0%, #27ae60 100%);
            color: white;
            padding: 30px 20px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
            font-weight: 700;
        }}
        .content {{
            padding: 30px 20px;
        }}
        .booking-ref {{
            background-color: #f8f9fa;
            border-left: 4px solid #2ecc71;
            padding: 15px;
            margin: 20px 0;
            font-size: 16px;
        }}
        .booking-ref strong {{
            color: #27ae60;
            font-size: 20px;
        }}
        .details {{
            background-color: #f8f9fa;
            border-radius: 8px;
            padding: 20px;
            margin: 20px 0;
        }}
        .details h2 {{
            margin-top: 0;
            color: #27ae60;
            font-size: 20px;
            border-bottom: 2px solid #27ae60;
            padding-bottom: 10px;
        }}
        .row {{
            display: flex;
            justify-content: space-between;
            padding: 12px 0;
            border-bottom: 1px solid #e0e0e0;
        }}
        .row:last-child {{
            border-bottom: none;
        }}
        .label {{
            font-weight: 600;
            color: #555;
        }}
        .value {{
            color: #333;
            text-align: right;
        }}
        .price-section {{
            background: linear-gradient(135deg, #2ecc71 0%, #27ae60 100%);
            color: white;
            padding: 20px;
            border-radius: 8px;
            margin: 20px 0;
        }}
        .price-section .label {{
            font-size: 14px;
            opacity: 0.95;
        }}
        .price-section .amount {{
            font-size: 32px;
            font-weight: bold;
            margin: 10px 0;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #666;
            font-size: 14px;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>✅ Payment Successful!</h1>
            <p>Your booking payment has been completed successfully</p>
        </div>

        <div class='content'>
            <p>Dear <strong>{passengerName}</strong>,</p>
            <p>We have received your payment successfully. Your booking is now marked as <strong>PAID</strong>.</p>

            <div class='booking-ref'>
                <strong>Booking Reference: {bookingReference}</strong>
            </div>

            <div class='details'>
                <h2>Flight Details</h2>
                <div class='row'><span class='label'>Flight Number:</span><span class='value'>{flightNumber}</span></div>
                <div class='row'><span class='label'>From:</span><span class='value'>{origin}</span></div>
                <div class='row'><span class='label'>To:</span><span class='value'>{destination}</span></div>
                <div class='row'><span class='label'>Departure:</span><span class='value'>{departureDate:dddd, MMMM dd, yyyy} {departureDate:hh:mm tt}</span></div>
            </div>

            <div class='price-section'>
                <div class='label'>Paid Amount</div>
                <div class='amount'>${totalAmount:N2}</div>
            </div>

            <p style='color: #666; font-size: 13px; margin-top: 15px;'>
                This is an automated email. Please do not reply.
            </p>
        </div>

        <div class='footer'>
            <p><strong>AeroNexa</strong></p>
            <p>© {DateTime.Now.Year} All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        // ✅ BOOKING CANCELLED HTML (RED THEME)
        private string GetBookingCancelledHtml(
            string passengerName,
            string flightNumber,
            string origin,
            string destination,
            DateTime departureDate,
            string bookingReference)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Booking Cancelled</title>
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
            background: linear-gradient(135deg, #e74c3c 0%, #c0392b 100%);
            color: white;
            padding: 30px 20px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
            font-weight: 700;
        }}
        .content {{
            padding: 30px 20px;
        }}
        .booking-ref {{
            background-color: #f8f9fa;
            border-left: 4px solid #e74c3c;
            padding: 15px;
            margin: 20px 0;
            font-size: 16px;
        }}
        .booking-ref strong {{
            color: #c0392b;
            font-size: 20px;
        }}
        .details {{
            background-color: #f8f9fa;
            border-radius: 8px;
            padding: 20px;
            margin: 20px 0;
        }}
        .details h2 {{
            margin-top: 0;
            color: #c0392b;
            font-size: 20px;
            border-bottom: 2px solid #c0392b;
            padding-bottom: 10px;
        }}
        .row {{
            display: flex;
            justify-content: space-between;
            padding: 12px 0;
            border-bottom: 1px solid #e0e0e0;
        }}
        .row:last-child {{
            border-bottom: none;
        }}
        .label {{
            font-weight: 600;
            color: #555;
        }}
        .value {{
            color: #333;
            text-align: right;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #666;
            font-size: 14px;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>❌ Booking Cancelled</h1>
            <p>Your booking has been cancelled successfully</p>
        </div>

        <div class='content'>
            <p>Dear <strong>{passengerName}</strong>,</p>
            <p>Your booking has been cancelled successfully.</p>

            <div class='booking-ref'>
                <strong>Booking Reference: {bookingReference}</strong>
            </div>

            <div class='details'>
                <h2>Cancelled Flight Details</h2>
                <div class='row'><span class='label'>Flight Number:</span><span class='value'>{flightNumber}</span></div>
                <div class='row'><span class='label'>From:</span><span class='value'>{origin}</span></div>
                <div class='row'><span class='label'>To:</span><span class='value'>{destination}</span></div>
                <div class='row'><span class='label'>Departure:</span><span class='value'>{departureDate:dddd, MMMM dd, yyyy} {departureDate:hh:mm tt}</span></div>
            </div>

            <p style='color: #666; font-size: 13px; margin-top: 15px;'>
                This is an automated email. Please do not reply.
            </p>
        </div>

        <div class='footer'>
            <p><strong>AeroNexa</strong></p>
            <p>© {DateTime.Now.Year} All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
