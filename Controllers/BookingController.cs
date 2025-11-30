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

        // small helper to fill dropdown lists
        private void PopulateDropdowns(Booking booking)
        {
            ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber", booking.FlightId);
            ViewBag.PassengerId = new SelectList(_context.Passengers, "Id", "FullName", booking.PassengerId);
            ViewBag.SeatClass = new SelectList(Enum.GetValues(typeof(SeatClass)), booking.SeatClass);
            ViewBag.PassengerType = new SelectList(Enum.GetValues(typeof(PassengerType)), booking.PassengerType);
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .ToListAsync();

            return View(bookings);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // CREATE GET
        public IActionResult Create()
        {
            PopulateDropdowns(new Booking());
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            // generate reference first so [Required] on BookingReference (if any) will pass
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

            // prevent booking with past date
            if (booking.BookingDate < DateTime.Now)
            {
                ModelState.AddModelError("BookingDate", "Booking date cannot be in the past.");
                PopulateDropdowns(booking);
                return View(booking);
            }

            // prevent booking after flight departure
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

            // seat availability check
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

            // deduct seats
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

            // assign user id (simple)
            booking.UserId = User?.Identity?.IsAuthenticated == true
                ? (User.Identity.Name ?? "USER")
                : "GUEST";

            _context.Flights.Update(flight);
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("Summary", new { id = booking.Id });
        }

        private string GenerateBookingReference()
        {
            return "BR-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            PopulateDropdowns(booking);
            return View(booking);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
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

            // If class or seat count changes, adjust flight availability
            if (existing.SeatClass != booking.SeatClass || existing.SeatCount != booking.SeatCount)
            {
                // restore previous seats on old flight
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

                // check new availability on same flight (or we could support changing flights too)
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

                // deduct new seats
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

            // recalc price
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

            // update editable fields
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

        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            // restore seats back to flight based on class
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



        // SUMMARY
        public async Task<IActionResult> Summary(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

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
    }
}
