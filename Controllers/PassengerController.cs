using Microsoft.AspNetCore.Mvc;
using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;

namespace AirlineTicketingSystem.Controllers
{
    public class PassengerController : Controller
    {
        private readonly DatabaseContext _context;

        public PassengerController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Passenger
        public IActionResult Index()
        {
            var passengers = _context.Passengers.ToList();
            return View(passengers);
        }

        // GET: Passenger/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Passenger/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Passenger passenger)
        {
            if (ModelState.IsValid)
            {
                _context.Passengers.Add(passenger);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(passenger);
        }

        // GET: Passenger/Edit/5
        public IActionResult Edit(int id)
        {
            var passenger = _context.Passengers.FirstOrDefault(x => x.Id == id);
            if (passenger == null) return NotFound();
            return View(passenger);
        }

        // POST: Passenger/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Passenger passenger)
        {
            if (ModelState.IsValid)
            {
                _context.Passengers.Update(passenger);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(passenger);
        }

        // GET: Passenger/Delete/5
        public IActionResult Delete(int id)
        {
            var passenger = _context.Passengers.FirstOrDefault(x => x.Id == id);
            if (passenger == null) return NotFound();
            return View(passenger);
        }

        // POST: Passenger/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var passenger = _context.Passengers.FirstOrDefault(x => x.Id == id);
            if (passenger == null) return NotFound();

            _context.Passengers.Remove(passenger);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Passenger/Details/5
        public IActionResult Details(int id)
        {
            var passenger = _context.Passengers.FirstOrDefault(x => x.Id == id);
            if (passenger == null) return NotFound();
            return View(passenger);
        }
    }
}
