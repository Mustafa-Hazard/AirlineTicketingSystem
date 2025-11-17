using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IActionResult Index()
        {
            var bookings = _context.Bookings
                .ToList(); // you can include Flight & Passenger later using Include()
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
        public IActionResult Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber");
            ViewBag.PassengerId = new SelectList(_context.Passengers, "Id", "FullName");

            return View(booking);
        }

        // GET: Booking/Edit/5
        public IActionResult Edit(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.Id == id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Bookings.Update(booking);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        // GET: Booking/Delete/5
        public IActionResult Delete(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.Id == id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.Id == id);
            if (booking == null) return NotFound();

            _context.Bookings.Remove(booking);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Booking/Details/5
        public IActionResult Details(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.Id == id);
            if (booking == null) return NotFound();
            return View(booking);
        }
    }
}
