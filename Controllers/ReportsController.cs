using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AirlineTicketingSystem.Controllers
{
    [Authorize(Roles = "Admin")] // Only Admin can access all actions in this controller
    public class ReportsController : Controller
    {
        private readonly DatabaseContext _context;

        public ReportsController(DatabaseContext context)
        {
            _context = context;
        }

        // ========== 1) BOOKINGS PER FLIGHT REPORT ========== 
        // Optional Date Filter
        public async Task<IActionResult> BookingsPerFlight(DateTime? startDate, DateTime? endDate)
        {
            var flights = await _context.Flights
                .Include(f => f.Bookings)
                .Where(f => (!startDate.HasValue || f.DepartureTime >= startDate) &&
                            (!endDate.HasValue || f.DepartureTime <= endDate))
                .ToListAsync();

            var rows = flights
                .Select(f => new FlightReportRowVM
                {
                    FlightId = f.Id,
                    FlightNumber = f.FlightNumber,
                    Origin = f.Origin,
                    Destination = f.Destination,
                    DepartureTime = f.DepartureTime,
                    BookingCount = f.Bookings?.Count ?? 0,
                    SeatsBooked = f.Bookings?.Sum(b => b.SeatCount) ?? 0,
                    PaidRevenue = f.Bookings?
                        .Where(b => b.IsPaid)
                        .Sum(b => b.TotalPrice) ?? 0m
                })
                .OrderByDescending(r => r.BookingCount)
                .ThenBy(r => r.DepartureTime)
                .ToList();

            return View(rows);
        }

        // ========== 2) REVENUE STATISTICS REPORT ========== 
        // Optional Date Filter
        public async Task<IActionResult> RevenueStatistics(DateTime? startDate, DateTime? endDate)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Flight)
                .Where(b => (!startDate.HasValue || b.Flight.DepartureTime >= startDate) &&
                            (!endDate.HasValue || b.Flight.DepartureTime <= endDate))
                .ToListAsync();

            var totalBookings = bookings.Count;
            var paidBookings = bookings.Count(b => b.IsPaid);
            var unpaidBookings = totalBookings - paidBookings;

            var totalPaidRevenue = bookings
                .Where(b => b.IsPaid)
                .Sum(b => b.TotalPrice);

            var totalUnpaidPotential = bookings
                .Where(b => !b.IsPaid)
                .Sum(b => b.TotalPrice);

            var perFlight = bookings
                .GroupBy(b => b.FlightId)
                .Select(g =>
                {
                    var first = g.First();
                    return new FlightReportRowVM
                    {
                        FlightId = g.Key,
                        FlightNumber = first.Flight?.FlightNumber ?? "",
                        Origin = first.Flight?.Origin ?? "",
                        Destination = first.Flight?.Destination ?? "",
                        DepartureTime = first.Flight?.DepartureTime ?? DateTime.MinValue,
                        BookingCount = g.Count(),
                        SeatsBooked = g.Sum(b => b.SeatCount),
                        PaidRevenue = g.Where(b => b.IsPaid).Sum(b => b.TotalPrice)
                    };
                })
                .OrderByDescending(r => r.PaidRevenue)
                .ToList();

            var vm = new RevenueSummaryVM
            {
                TotalBookings = totalBookings,
                PaidBookings = paidBookings,
                UnpaidBookings = unpaidBookings,
                TotalPaidRevenue = totalPaidRevenue,
                TotalUnpaidPotential = totalUnpaidPotential,
                PerFlight = perFlight
            };

            return View(vm);
        }
    }
}
