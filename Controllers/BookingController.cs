using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;
using AirlineTicketingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AirlineTicketingSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly DatabaseContext _context;

        public BookingController(DatabaseContext context)
        {
            _context = context;
        }

        // ------------------------- INDEX -------------------------
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .ToListAsync();

            return View(bookings);
        }

        // ------------------------- CREATE GET -------------------------
        public IActionResult Create()
        {
            ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber");
            ViewBag.PassengerId = new SelectList(_context.Passengers, "Id", "FullName");

            return View();
        }

        // ------------------------- CREATE POST -------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber", booking.FlightId);
                ViewBag.PassengerId = new SelectList(_context.Passengers, "Id", "FullName", booking.PassengerId);
                return View(booking);
            }

            // -------- Generate Booking Reference --------
            booking.BookingReference = GenerateBookingReference();

            // -------- Fetch Flight for Validation --------
            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == booking.FlightId);

            if (flight == null)
            {
                ModelState.AddModelError("", "Selected flight not found.");
                return View(booking);
            }

            // -------- Check Seat Availability --------
            if (booking.SeatCount > flight.AvailableSeats)
            {
                ModelState.AddModelError("SeatCount", $"Only {flight.AvailableSeats} seats remaining.");
                ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber", booking.FlightId);
                ViewBag.PassengerId = new SelectList(_context.Passengers, "Id", "FullName", booking.PassengerId);
                return View(booking);
            }

            // -------- Calculate Total Price --------
            booking.TotalPrice = booking.SeatCount * flight.Price;

            // -------- Assign User ID (placeholder until Identity is added) --------
            booking.UserId = "TEMP-USER"; // TODO: Replace when Identity added

            // -------- Reduce Seats from Flight --------
            flight.AvailableSeats -= booking.SeatCount;
            _context.Flights.Update(flight);

            // -------- Save Booking --------
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("Summary", new { id = booking.Id });
        }

        private string GenerateBookingReference()
        {
            return "BR-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        // ------------------------- EDIT GET -------------------------
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber", booking.FlightId);
            ViewBag.PassengerId = new SelectList(_context.Passengers, "Id", "FullName", booking.PassengerId);

            return View(booking);
        }

        // ------------------------- EDIT POST -------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber", booking.FlightId);
                ViewBag.PassengerId = new SelectList(_context.Passengers, "Id", "FullName", booking.PassengerId);
                return View(booking);
            }

            var existing = await _context.Bookings.FindAsync(booking.Id);
            if (existing == null) return NotFound();

            // We do NOT change seat count here (advanced logic)
            existing.PassengerId = booking.PassengerId;
            existing.FlightId = booking.FlightId;
            existing.IsPaid = booking.IsPaid;
            existing.BookingDate = booking.BookingDate;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ------------------------- DELETE GET -------------------------
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // ------------------------- DELETE POST -------------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            // restore seats
            booking.Flight.AvailableSeats += booking.SeatCount;
            _context.Flights.Update(booking.Flight);

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ------------------------- DETAILS -------------------------
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }
        public async Task<IActionResult> Summary(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            var vm = new BookingSummaryViewModel
            {
                BookingReference = booking.BookingReference,
                PassengerName = booking.Passenger?.FullName ?? "",
                FlightNumber = booking.Flight?.FlightNumber ?? "",

                From = booking.Flight?.FromAirport ?? "",
                To = booking.Flight?.ToAirport ?? "",

                DepartureTimeTime = booking.Flight!.DepartureTime,

                SeatCount = booking.SeatCount,
                PricePerSeat = booking.Flight.Price,
                TotalPrice = booking.TotalPrice,

                IsPaid = booking.IsPaid
            };

            return View(vm);
        }
    }
}
