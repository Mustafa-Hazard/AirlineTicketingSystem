using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;
using AirlineTicketingSystem.Models.ViewModels;
using AirlineTicketingSystem.Pdf;
using AirlineTicketingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Security.Claims;

namespace AirlineTicketingSystem.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IEmailService _emailService;

        public BookingController(DatabaseContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Helper to fill dropdown lists - NOW FILTERED BY USER
        private void PopulateDropdowns(Booking booking)
        {
            ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber", booking.FlightId);

            // SECURITY FIX: Only show passengers belonging to current user
            var currentUserName = User.Identity.Name;
            var userPassengers = _context.Passengers
                .Where(p => p.UserId == currentUserName)
                .ToList();

            ViewBag.PassengerId = new SelectList(userPassengers, "Id", "FullName", booking.PassengerId);
            ViewBag.SeatClass = new SelectList(Enum.GetValues(typeof(SeatClass)), booking.SeatClass);
            ViewBag.PassengerType = new SelectList(Enum.GetValues(typeof(PassengerType)), booking.PassengerType);
        }

        // ========== INDEX (ROLE-BASED) ==========
        public async Task<IActionResult> Index()
        {
            var currentUserName = User.Identity.Name;

            if (User.IsInRole("Admin"))
            {
                // Admin sees ALL bookings
                var allBookings = await _context.Bookings
                    .Include(b => b.Flight)
                    .Include(b => b.Passenger)
                    .ToListAsync();
                return View(allBookings);
            }
            else
            {
                // Customer sees ONLY THEIR bookings
                var myBookings = await _context.Bookings
                    .Include(b => b.Flight)
                    .Include(b => b.Passenger)
                    .Where(b => b.UserId == currentUserName)
                    .OrderByDescending(b => b.BookingDate)
                    .ToListAsync();
                return View("MyBookings", myBookings);
            }
        }

        // ========== MY BOOKINGS (CUSTOMER ONLY) ==========
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyBookings()
        {
            var currentUserName = User.Identity.Name;

            var myBookings = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .Where(b => b.UserId == currentUserName)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(myBookings);
        }

        // ========== DETAILS (SECURITY CHECKED) ==========
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            // SECURITY CHECK: Customers can only see their own bookings
            var currentUserName = User.Identity.Name;
            if (User.IsInRole("Customer") && booking.UserId != currentUserName)
            {
                return Forbid();
            }

            return View(booking);
        }

        // ========== CREATE ==========
        public IActionResult Create()
        {
            PopulateDropdowns(new Booking());

            // Show message if user has no passengers
            var currentUserName = User.Identity.Name;
            var hasPassengers = _context.Passengers.Any(p => p.UserId == currentUserName);

            if (!hasPassengers)
            {
                TempData["Info"] = "You need to create a passenger first before booking.";
            }

            return View();
        }

        // ========== CREATE POST (WITH EMAIL CONFIRMATION) ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            booking.BookingReference = GenerateBookingReference();

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(booking);
                return View(booking);
            }

            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == booking.FlightId);
            if (flight == null)
            {
                ModelState.AddModelError("", "Selected flight not found.");
                PopulateDropdowns(booking);
                return View(booking);
            }

            // SECURITY CHECK: Verify passenger belongs to current user
            var currentUserName = User.Identity.Name;
            var passenger = await _context.Passengers.FindAsync(booking.PassengerId);

            if (passenger == null)
            {
                ModelState.AddModelError("PassengerId", "Invalid passenger selected.");
                PopulateDropdowns(booking);
                return View(booking);
            }

            // Only customers need this check (admins can book for anyone)
            if (User.IsInRole("Customer") && passenger.UserId != currentUserName)
            {
                ModelState.AddModelError("PassengerId", "You can only book for your own passengers.");
                PopulateDropdowns(booking);
                return View(booking);
            }

            if (booking.BookingDate < DateTime.Now)
            {
                ModelState.AddModelError("BookingDate", "Booking date cannot be in the past.");
                PopulateDropdowns(booking);
                return View(booking);
            }

            if (flight.DepartureTime <= DateTime.Now)
            {
                ModelState.AddModelError("", "This flight has already departed. Cannot book.");
                PopulateDropdowns(booking);
                return View(booking);
            }

            // Price calculation
            decimal basePrice = booking.SeatClass switch
            {
                SeatClass.Economy => flight.EconomyPrice,
                SeatClass.Business => flight.BusinessPrice,
                SeatClass.FirstClass => flight.FirstClassPrice,
                _ => flight.EconomyPrice
            };

            decimal multiplier = booking.PassengerType switch
            {
                PassengerType.Adult => 1.0m,
                PassengerType.Child => 0.7m,
                PassengerType.Infant => 0.2m,
                _ => 1.0m
            };

            decimal pricePerSeat = basePrice * multiplier;

            // Seat availability check
            bool notEnough = booking.SeatClass switch
            {
                SeatClass.Economy => booking.SeatCount > flight.AvailableEconomySeats,
                SeatClass.Business => booking.SeatCount > flight.AvailableBusinessSeats,
                SeatClass.FirstClass => booking.SeatCount > flight.AvailableFirstClassSeats,
                _ => true
            };

            if (notEnough)
            {
                ModelState.AddModelError("SeatCount", "Not enough seats available in the selected class.");
                PopulateDropdowns(booking);
                return View(booking);
            }

            // Deduct seats
            switch (booking.SeatClass)
            {
                case SeatClass.Economy:
                    flight.AvailableEconomySeats -= booking.SeatCount;
                    break;
                case SeatClass.Business:
                    flight.AvailableBusinessSeats -= booking.SeatCount;
                    break;
                case SeatClass.FirstClass:
                    flight.AvailableFirstClassSeats -= booking.SeatCount;
                    break;
            }

            booking.TotalPrice = pricePerSeat * booking.SeatCount;
            booking.UserId = currentUserName;

            _context.Flights.Update(flight);
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // ============ EMAIL CONFIRMATION ============
            try
            {
                await _emailService.SendBookingConfirmationAsync(
                    passenger.ContactEmail,
                    passenger.FullName,
                    flight.FlightNumber,
                    flight.Destination,
                    flight.DepartureTime,
                    booking.BookingReference,
                    booking.TotalPrice
                );
                TempData["Success"] = "✅ Booking confirmed successfully! Confirmation email has been sent.";
            }
            catch (Exception ex)
            {
                TempData["Warning"] = "⚠️ Booking confirmed but email could not be sent. Please check your email settings.";
            }

            return RedirectToAction("Summary", new { id = booking.Id });
        }

        private string GenerateBookingReference()
        {
            return "BR-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        // ========== EDIT (ADMIN ONLY) ==========
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            PopulateDropdowns(booking);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(booking);
                return View(booking);
            }

            var existing = await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == booking.Id);

            if (existing == null) return NotFound();

            if (existing.SeatClass != booking.SeatClass || existing.SeatCount != booking.SeatCount)
            {
                var flight = await _context.Flights.FindAsync(existing.FlightId);
                if (flight == null) return BadRequest();

                switch (existing.SeatClass)
                {
                    case SeatClass.Economy:
                        flight.AvailableEconomySeats += existing.SeatCount;
                        break;
                    case SeatClass.Business:
                        flight.AvailableBusinessSeats += existing.SeatCount;
                        break;
                    case SeatClass.FirstClass:
                        flight.AvailableFirstClassSeats += existing.SeatCount;
                        break;
                }

                bool notEnough = booking.SeatClass switch
                {
                    SeatClass.Economy => booking.SeatCount > flight.AvailableEconomySeats,
                    SeatClass.Business => booking.SeatCount > flight.AvailableBusinessSeats,
                    SeatClass.FirstClass => booking.SeatCount > flight.AvailableFirstClassSeats,
                    _ => true
                };

                if (notEnough)
                {
                    ModelState.AddModelError("SeatCount", "Not enough seats available for the updated selection.");
                    PopulateDropdowns(booking);
                    return View(booking);
                }

                switch (booking.SeatClass)
                {
                    case SeatClass.Economy:
                        flight.AvailableEconomySeats -= booking.SeatCount;
                        break;
                    case SeatClass.Business:
                        flight.AvailableBusinessSeats -= booking.SeatCount;
                        break;
                    case SeatClass.FirstClass:
                        flight.AvailableFirstClassSeats -= booking.SeatCount;
                        break;
                }

                _context.Flights.Update(flight);
            }

            var selectedFlight = await _context.Flights.FindAsync(booking.FlightId);
            if (selectedFlight == null) return BadRequest();

            decimal basePrice = booking.SeatClass switch
            {
                SeatClass.Economy => selectedFlight.EconomyPrice,
                SeatClass.Business => selectedFlight.BusinessPrice,
                SeatClass.FirstClass => selectedFlight.FirstClassPrice,
                _ => selectedFlight.EconomyPrice
            };

            decimal multiplier = booking.PassengerType switch
            {
                PassengerType.Adult => 1.0m,
                PassengerType.Child => 0.7m,
                PassengerType.Infant => 0.2m,
                _ => 1.0m
            };

            booking.TotalPrice = basePrice * multiplier * booking.SeatCount;

            existing.FlightId = booking.FlightId;
            existing.PassengerId = booking.PassengerId;
            existing.SeatCount = booking.SeatCount;
            existing.SeatClass = booking.SeatClass;
            existing.PassengerType = booking.PassengerType;
            existing.IsPaid = booking.IsPaid;
            existing.TotalPrice = booking.TotalPrice;
            existing.BookingDate = booking.BookingDate;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ========== DELETE (ADMIN ONLY) ==========
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            var flight = booking.Flight;
            if (flight != null)
            {
                switch (booking.SeatClass)
                {
                    case SeatClass.Economy:
                        flight.AvailableEconomySeats += booking.SeatCount;
                        break;
                    case SeatClass.Business:
                        flight.AvailableBusinessSeats += booking.SeatCount;
                        break;
                    case SeatClass.FirstClass:
                        flight.AvailableFirstClassSeats += booking.SeatCount;
                        break;
                }
                _context.Flights.Update(flight);
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            if (!booking.IsPaid)
            {
                booking.IsPaid = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Summary", new { id = booking.Id });
        }

        // ========== SUMMARY (SECURITY CHECKED) ==========
        public async Task<IActionResult> Summary(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            // SECURITY CHECK: Customers can only see their own booking summaries
            var currentUserName = User.Identity.Name;
            if (User.IsInRole("Customer") && booking.UserId != currentUserName)
            {
                return Forbid();
            }

            var vm = new BookingSummaryViewModel
            {
                Id = booking.Id,
                BookingReference = booking.BookingReference,
                PassengerName = booking.Passenger?.FullName ?? "",
                FlightNumber = booking.Flight?.FlightNumber ?? "",
                From = booking.Flight?.Origin ?? "",
                To = booking.Flight?.Destination ?? "",
                DepartureTime = booking.Flight?.DepartureTime ?? DateTime.MinValue,
                SeatCount = booking.SeatCount,
                PricePerSeat = booking.SeatCount > 0
                    ? booking.TotalPrice / booking.SeatCount
                    : booking.TotalPrice,
                TotalPrice = booking.TotalPrice,
                IsPaid = booking.IsPaid,
                SeatClass = booking.SeatClass.ToString(),
                PassengerType = booking.PassengerType.ToString()
            };

            return View(vm);
        }

        // ========== E-TICKET PDF (SECURITY CHECKED) ==========
        [HttpGet]
        public async Task<IActionResult> ETicket(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            // SECURITY CHECK: Customers can only download their own tickets
            var currentUserName = User.Identity.Name;
            if (User.IsInRole("Customer") && booking.UserId != currentUserName)
            {
                return Forbid();
            }

            var vm = new BookingSummaryViewModel
            {
                Id = booking.Id,
                BookingReference = booking.BookingReference,
                PassengerName = booking.Passenger?.FullName ?? "",
                FlightNumber = booking.Flight?.FlightNumber ?? "",
                From = booking.Flight?.Origin ?? "",
                To = booking.Flight?.Destination ?? "",
                DepartureTime = booking.Flight?.DepartureTime ?? DateTime.MinValue,
                SeatCount = booking.SeatCount,
                PricePerSeat = booking.SeatCount > 0
                    ? booking.TotalPrice / booking.SeatCount
                    : booking.TotalPrice,
                TotalPrice = booking.TotalPrice,
                IsPaid = booking.IsPaid,
                SeatClass = booking.SeatClass.ToString(),
                PassengerType = booking.PassengerType.ToString()
            };

            var document = new ETicketDocument(vm);
            var pdfBytes = document.GeneratePdf();

            var fileName = $"ETicket_{vm.BookingReference}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}