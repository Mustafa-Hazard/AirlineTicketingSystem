using System;
using System.Collections.Generic;

namespace AirlineTicketingSystem.Models.ViewModels
{
    public class RevenueSummaryVM
    {
        public int TotalBookings { get; set; }
        public int PaidBookings { get; set; }
        public int UnpaidBookings { get; set; }

        public decimal TotalPaidRevenue { get; set; }
        public decimal TotalUnpaidPotential { get; set; }

        public List<FlightReportRowVM> PerFlight { get; set; } = new();
    }
}
