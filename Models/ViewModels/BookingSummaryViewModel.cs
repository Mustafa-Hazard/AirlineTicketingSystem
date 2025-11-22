namespace AirlineTicketingSystem.Models.ViewModels
{
    public class BookingSummaryViewModel
    {
        public string BookingReference { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;

        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;

        public DateTime DepartureTimeTime { get; set; }

        public int SeatCount { get; set; }
        public decimal PricePerSeat { get; set; }
        public decimal TotalPrice { get; set; }

        public bool IsPaid { get; set; }
    }
}
