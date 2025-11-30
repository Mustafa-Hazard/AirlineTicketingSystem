using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirlineTicketingSystem.ViewModels;


namespace AirlineTicketingSystem.Controllers
{
    public class FlightsController : Controller
    {
        private readonly DatabaseContext _context;

        public FlightsController(DatabaseContext context)
        {
            _context = context;
        }
        public IActionResult ViewAllWithBookings()
        {
            var flights = _context.Flights.Include(f => f.Bookings)
              .ThenInclude(b => b.Passenger)
                .ToList();

            return View(flights);
        }
        // INDEX
        public async Task<IActionResult> Index()
        {
            var flights = await _context.Flights.ToListAsync();
            return View(flights);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        // CREATE GET
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Flight model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // VALIDATION: ARRIVAL > DEPARTURE
            if (model.ArrivalTime <= model.DepartureTime)
            {
                ModelState.AddModelError("ArrivalTime", "Arrival time must be later than departure time.");
                return View(model);
            }

            // initialize available seats from totals
            model.AvailableEconomySeats = model.EconomySeats;
            model.AvailableBusinessSeats = model.BusinessSeats;
            model.AvailableFirstClassSeats = model.FirstClassSeats;

            _context.Flights.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Flight model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // VALIDATION MUST BE HERE
            if (model.ArrivalTime <= model.DepartureTime)
            {
                ModelState.AddModelError("ArrivalTime", "Arrival time must be later than departure time.");
                return View(model);
            }

            var existing = await _context.Flights.FindAsync(model.Id);
            if (existing == null) return NotFound();

            existing.FlightNumber = model.FlightNumber.Trim();
            existing.Origin = model.Origin.Trim();
            existing.Destination = model.Destination.Trim();
            existing.FromAirport = model.FromAirport?.Trim();
            existing.ToAirport = model.ToAirport?.Trim();
            existing.DepartureTime = model.DepartureTime;
            existing.ArrivalTime = model.ArrivalTime;

            existing.EconomySeats = model.EconomySeats;
            existing.AvailableEconomySeats = Math.Min(existing.AvailableEconomySeats, model.EconomySeats);
            existing.BusinessSeats = model.BusinessSeats;
            existing.AvailableBusinessSeats = Math.Min(existing.AvailableBusinessSeats, model.BusinessSeats);
            existing.FirstClassSeats = model.FirstClassSeats;
            existing.AvailableFirstClassSeats = Math.Min(existing.AvailableFirstClassSeats, model.FirstClassSeats);

            existing.EconomyPrice = model.EconomyPrice;
            existing.BusinessPrice = model.BusinessPrice;
            existing.FirstClassPrice = model.FirstClassPrice;

            existing.IsDelayed = model.IsDelayed;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // SEARCH (GET)
        public IActionResult Search(FlightSearchVM vm)
        {
            var query = _context.Flights.AsQueryable();

            if (!string.IsNullOrWhiteSpace(vm.Origin))
                query = query.Where(f => f.Origin.Contains(vm.Origin));

            if (!string.IsNullOrWhiteSpace(vm.Destination))
                query = query.Where(f => f.Destination.Contains(vm.Destination));

            if (vm.DepartureDate.HasValue)
            {
                var date = vm.DepartureDate.Value.Date;
                query = query.Where(f => f.DepartureTime.Date == date);
            }

            vm.Results = query
                .OrderBy(f => f.DepartureTime)
                .ToList();

            return View(vm);
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            var flight = await _context.Flights
                .Include(f => f.Bookings)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null) return NotFound();
            return View(flight);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights
                .Include(f => f.Bookings)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null) return NotFound();

            if (flight.Bookings != null && flight.Bookings.Any())
            {
                TempData["Error"] = "Cannot delete flight that has bookings.";
                return RedirectToAction(nameof(Index));
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
