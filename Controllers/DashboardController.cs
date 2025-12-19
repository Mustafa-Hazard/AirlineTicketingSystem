using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.ViewModels;
using AirlineTicketingSystem.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirlineTicketingSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DatabaseContext _context;

        public DashboardController(DatabaseContext context)
        {
            _context = context;
        }

        // Main Dashboard - Redirects based on role
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminDashboard");
            }
            else
            {
                return RedirectToAction("CustomerDashboard");
            }
        }

        // Admin Dashboard
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var today = DateTime.Today;

            // Get statistics
            var totalFlights = await _context.Flights.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();
            var totalPassengers = await _context.Passengers.CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            // Bookings statistics
            var paidBookings = await _context.Bookings.CountAsync(b => b.IsPaid);
            var unpaidBookings = totalBookings - paidBookings;

            // Revenue statistics
            var totalRevenue = await _context.Bookings
                .Where(b => b.IsPaid)
                .SumAsync(b => (decimal?)b.TotalPrice) ?? 0;

            var potentialRevenue = await _context.Bookings
                .Where(b => !b.IsPaid)
                .SumAsync(b => (decimal?)b.TotalPrice) ?? 0;

            // Recent bookings (last 5)
            var recentBookings = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            // Upcoming flights (next 5)
            var upcomingFlights = await _context.Flights
                .Where(f => f.DepartureTime > DateTime.Now && !f.IsCancelled)
                .OrderBy(f => f.DepartureTime)
                .Take(5)
                .ToListAsync();

            // Popular routes (top 5 by bookings)
            var popularRoutes = await _context.Bookings
                .Include(b => b.Flight)
                .GroupBy(b => new { b.Flight.Origin, b.Flight.Destination })
                .Select(g => new PopularRouteVM
                {
                    Route = g.Key.Origin + " → " + g.Key.Destination,
                    BookingCount = g.Count(),
                    Revenue = g.Where(b => b.IsPaid).Sum(b => (decimal?)b.TotalPrice) ?? 0
                })
                .OrderByDescending(x => x.BookingCount)
                .Take(5)
                .ToListAsync();

            // Seat class distribution
            var seatClassStats = await _context.Bookings
                .GroupBy(b => b.SeatClass)
                .Select(g => new SeatClassStatVM
                {
                    SeatClass = g.Key.ToString(),
                    Count = g.Count(),
                    Revenue = g.Where(b => b.IsPaid).Sum(b => (decimal?)b.TotalPrice) ?? 0
                })
                .ToListAsync();

            // Today's statistics
            var todayBookings = await _context.Bookings
                .CountAsync(b => b.BookingDate.Date == today);

            var todayRevenue = await _context.Bookings
                .Where(b => b.BookingDate.Date == today && b.IsPaid)
                .SumAsync(b => (decimal?)b.TotalPrice) ?? 0;

            // Flights departing today
            var todayFlights = await _context.Flights
                .CountAsync(f => f.DepartureTime.Date == today);

            // Create strongly-typed view model
            var model = new AdminDashboardVM
            {
                TotalFlights = totalFlights,
                TotalBookings = totalBookings,
                TotalPassengers = totalPassengers,
                TotalUsers = totalUsers,
                PaidBookings = paidBookings,
                UnpaidBookings = unpaidBookings,
                TotalRevenue = totalRevenue,
                PotentialRevenue = potentialRevenue,
                RecentBookings = recentBookings,
                UpcomingFlights = upcomingFlights,
                PopularRoutes = popularRoutes,
                SeatClassStats = seatClassStats,
                TodayBookings = todayBookings,
                TodayRevenue = todayRevenue,
                TodayFlights = todayFlights
            };

            return View(model);
        }

        // Customer Dashboard
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CustomerDashboard()
        {
            var currentUserName = User.Identity.Name;

            // Get customer's bookings
            var myBookings = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .Where(b => b.UserId == currentUserName)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            // Get customer's passengers
            var myPassengers = await _context.Passengers
                .Where(p => p.UserId == currentUserName)
                .ToListAsync();

            // Statistics
            var totalBookings = myBookings.Count;
            var upcomingTrips = myBookings.Count(b => b.Flight.DepartureTime > DateTime.Now);
            var completedTrips = myBookings.Count(b => b.Flight.DepartureTime <= DateTime.Now);
            var totalSpent = myBookings.Where(b => b.IsPaid).Sum(b => (decimal?)b.TotalPrice) ?? 0;
            var pendingPayments = myBookings.Where(b => !b.IsPaid).Sum(b => (decimal?)b.TotalPrice) ?? 0;

            // Upcoming bookings (next 3)
            var upcomingBookings = myBookings
                .Where(b => b.Flight.DepartureTime > DateTime.Now)
                .OrderBy(b => b.Flight.DepartureTime)
                .Take(3)
                .ToList();

            // Recent bookings (last 5)
            var recentBookings = myBookings
                .Take(5)
                .ToList();

            // Create strongly-typed view model
            var model = new CustomerDashboardVM
            {
                TotalBookings = totalBookings,
                UpcomingTrips = upcomingTrips,
                CompletedTrips = completedTrips,
                TotalSpent = totalSpent,
                PendingPayments = pendingPayments,
                TotalPassengers = myPassengers.Count,
                UpcomingBookings = upcomingBookings,
                RecentBookings = recentBookings,
                MyPassengers = myPassengers
            };

            return View(model);
        }
    }
}