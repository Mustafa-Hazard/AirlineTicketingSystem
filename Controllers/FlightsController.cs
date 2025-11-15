using Microsoft.AspNetCore.Mvc;
using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;
namespace Controllers
{
    public class FlightsController : Controller
    {
        private readonly DatabaseContext _context;

        public FlightsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Flights
        public IActionResult Index()
        {
            var flights = _context.Flights.ToList();
            return View(flights);
        }

        // GET: Flights/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Flights.Add(flight);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(flight);
        }

        // GET: Flights/Edit/5
        public IActionResult Edit(int id)
        {
            var flight = _context.Flights.FirstOrDefault(x => x.Id == id);
            if (flight == null) return NotFound();

            return View(flight);
        }

        // POST: Flights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Flights.Update(flight);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(flight);
        }

        // GET: Flights/Delete/5
        public IActionResult Delete(int id)
        {
            var flight = _context.Flights.FirstOrDefault(x => x.Id == id);
            if (flight == null) return NotFound();

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var flight = _context.Flights.FirstOrDefault(x => x.Id == id);
            if (flight == null) return NotFound();

            _context.Flights.Remove(flight);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Flights/Details/5
        public IActionResult Details(int id)
        {
            var flight = _context.Flights.FirstOrDefault(x => x.Id == id);
            if (flight == null) return NotFound();

            return View(flight);
        }
    }
}
