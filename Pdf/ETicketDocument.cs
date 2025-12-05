using AirlineTicketingSystem.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AirlineTicketingSystem.Pdf
{
    public class ETicketDocument : IDocument
    {
        public BookingSummaryViewModel Model { get; }

        public ETicketDocument(BookingSummaryViewModel model)
        {
            Model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    // Header
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("AeroNexa E-Ticket")
                                .FontSize(20).Bold();
                            c.Item().Text("Boarding Pass / Electronic Ticket");
                        });

                        row.ConstantItem(200).Column(c =>
                        {
                            c.Item().Text($"Booking Ref: {Model.BookingReference}").Bold();
                            c.Item().Text($"Status: {(Model.IsPaid ? "PAID" : "PENDING")}");
                        });
                    });

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    // Passenger info
                    col.Item().Text("Passenger Details").FontSize(14).Bold().Underline();
                    col.Item().Grid(grid =>
                    {
                        grid.Columns(2);

                        grid.Item().Text($"Name: {Model.PassengerName}");
                        grid.Item().Text($"Passenger Type: {Model.PassengerType}");
                    });

                    // simple vertical space
                    col.Item().Text(" ");

                    // Flight info
                    col.Item().Text("Flight Details").FontSize(14).Bold().Underline();
                    col.Item().Grid(grid =>
                    {
                        grid.Columns(2);

                        grid.Item().Text($"Flight: {Model.FlightNumber}");
                        grid.Item().Text($"Seat Class: {Model.SeatClass}");

                        grid.Item().Text($"From: {Model.From}");
                        grid.Item().Text($"To: {Model.To}");

                        grid.Item().Text($"Departure: {Model.DepartureTime:dd MMM yyyy, hh:mm tt}");
                    });

                    // simple vertical space
                    col.Item().Text(" ");

                    // Payment info
                    col.Item().Text("Payment Summary").FontSize(14).Bold().Underline();
                    col.Item().Grid(grid =>
                    {
                        grid.Columns(2);

                        grid.Item().Text($"Seats: {Model.SeatCount}");
                        grid.Item().Text($"Price per Seat: {Model.PricePerSeat:C}");

                        grid.Item().Text(string.Empty);
                        grid.Item().Text($"Total: {Model.TotalPrice:C}")
                            .FontSize(14).Bold();
                    });

                    // more space before info
                    col.Item().Text(" ");

                    col.Item().Text("Important Information").Bold();
                    col.Item().Text("• Please arrive at the airport at least 2 hours before departure.");
                    col.Item().Text("• Carry a valid photo ID along with this e-ticket.");
                    col.Item().Text("• Contact AeroNexa support for any changes or cancellations.");
                });

                page.Footer()
                    .AlignCenter()
                    .Text("Thank you for choosing AeroNexa")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken2);
            });
        }
    }
}
