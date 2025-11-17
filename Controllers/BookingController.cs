using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;
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

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .ToListAsync();
            return View(bookings);
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber");
            ViewBag.PassengerId = new SelectList(_context.Passengers, "Id", "FullName");
            return View();
        }

        // POST: Booking/Create
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

            // Optional: Check for duplicate booking reference
            if (await _context.Bookings.AnyAsync(b => b.BookingReference == booking.BookingReference))
            {
                ModelState.AddModelError("BookingReference", "Booking reference already exists.");
                ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber", booking.FlightId);
                ViewBag.PassengerId = new SelectList(_context.Passengers, "Id", "FullName", booking.PassengerId);
                return View(booking);
            }

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber", booking.FlightId);
            ViewBag.PassengerId = new SelectList(_context.Passengers, "Id", "FullName", booking.PassengerId);

            return View(booking);
        }

        // POST: Booking/Edit/5
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

            var existingBooking = await _context.Bookings.FindAsync(booking.Id);
            if (existingBooking == null) return NotFound();

            existingBooking.BookingReference = booking.BookingReference;
            existingBooking.FlightId = booking.FlightId;
            existingBooking.PassengerId = booking.PassengerId;
            existingBooking.UserId = booking.UserId;
            existingBooking.BookingDate = booking.BookingDate;
            existingBooking.TotalPrice = booking.TotalPrice;
            existingBooking.IsPaid = booking.IsPaid;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();
            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);
            
            if (booking == null) return NotFound();
            return View(booking);
        }
    }
}
