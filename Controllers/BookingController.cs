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

        // ✅ Admin Edit uses this (Passengers dropdown filtered by current user)
        private void PopulateDropdowns(Booking booking)
        {
            ViewBag.FlightId = new SelectList(_context.Flights, "Id", "FlightNumber", booking.FlightId);

            var currentUserName = User.Identity!.Name;
            var userPassengers = _context.Passengers
                .Where(p => p.UserId == currentUserName)
                .ToList();

            ViewBag.PassengerId = new SelectList(userPassengers, "Id", "FullName", booking.PassengerId);
            ViewBag.SeatClass = new SelectList(Enum.GetValues(typeof(SeatClass)), booking.SeatClass);
            ViewBag.PassengerType = new SelectList(Enum.GetValues(typeof(PassengerType)), booking.PassengerType);
        }

        private async Task<List<SelectListItem>> GetFlightsDropdownAsync()
        {
            return await _context.Flights
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.FlightNumber
                })
                .ToListAsync();
        }

        private static decimal GetBasePrice(Flight flight, SeatClass seatClass)
        {
            return seatClass switch
            {
                SeatClass.Economy => flight.EconomyPrice,
                SeatClass.Business => flight.BusinessPrice,
                SeatClass.FirstClass => flight.FirstClassPrice,
                _ => flight.EconomyPrice
            };
        }

        private static decimal GetPassengerMultiplier(PassengerType passengerType)
        {
            return passengerType switch
            {
                PassengerType.Adult => 1.0m,
                PassengerType.Child => 0.7m,
                PassengerType.Infant => 0.2m,
                _ => 1.0m
            };
        }

        private static void DeductSeats(Flight flight, SeatClass seatClass, int seatCount)
        {
            switch (seatClass)
            {
                case SeatClass.Economy: flight.AvailableEconomySeats -= seatCount; break;
                case SeatClass.Business: flight.AvailableBusinessSeats -= seatCount; break;
                case SeatClass.FirstClass: flight.AvailableFirstClassSeats -= seatCount; break;
            }
        }

        private static void RestoreSeats(Flight flight, SeatClass seatClass, int seatCount)
        {
            switch (seatClass)
            {
                case SeatClass.Economy: flight.AvailableEconomySeats += seatCount; break;
                case SeatClass.Business: flight.AvailableBusinessSeats += seatCount; break;
                case SeatClass.FirstClass: flight.AvailableFirstClassSeats += seatCount; break;
            }
        }

        private static bool HasEnoughSeats(Flight flight, SeatClass seatClass, int seatCount)
        {
            return seatClass switch
            {
                SeatClass.Economy => seatCount <= flight.AvailableEconomySeats,
                SeatClass.Business => seatCount <= flight.AvailableBusinessSeats,
                SeatClass.FirstClass => seatCount <= flight.AvailableFirstClassSeats,
                _ => false
            };
        }

        private async Task<string> GenerateUniqueBookingReferenceAsync()
        {
            // Keep generating until unique (simple, safe for small systems)
            while (true)
            {
                var candidate = "BR-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
                bool exists = await _context.Bookings.AnyAsync(b => b.BookingReference == candidate);
                if (!exists) return candidate;
            }
        }

        private static BookingSummaryViewModel ToSummaryVm(Booking booking)
        {
            return new BookingSummaryViewModel
            {
                Id = booking.Id,
                BookingReference = booking.BookingReference,
                PassengerName = booking.Passenger?.FullName ?? "",
                FlightNumber = booking.Flight?.FlightNumber ?? "",
                From = booking.Flight?.Origin ?? "",
                To = booking.Flight?.Destination ?? "",
                DepartureTime = booking.Flight?.DepartureTime ?? DateTime.MinValue,
                SeatCount = booking.SeatCount,
                PricePerSeat = booking.SeatCount > 0 ? booking.TotalPrice / booking.SeatCount : booking.TotalPrice,
                TotalPrice = booking.TotalPrice,
                IsPaid = booking.IsPaid,
                SeatClass = booking.SeatClass.ToString(),
                PassengerType = booking.PassengerType.ToString()
            };
        }

        // =========================
        // INDEX (Admin: all, Customer: my)
        // =========================
        public async Task<IActionResult> Index()
        {
            var currentUserName = User.Identity!.Name;

            if (User.IsInRole("Admin"))
            {
                var allBookings = await _context.Bookings
                    .Include(b => b.Flight)
                    .Include(b => b.Passenger)
                    .OrderByDescending(b => b.BookingDate)
                    .ToListAsync();

                return View(allBookings);
            }

            var myBookings = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .Where(b => b.UserId == currentUserName)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View("MyBookings", myBookings);
        }

        // =========================
        // MY BOOKINGS (Customer only)
        // =========================
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyBookings()
        {
            var currentUserName = User.Identity!.Name;

            var myBookings = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .Where(b => b.UserId == currentUserName)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(myBookings);
        }

        // =========================
        // DETAILS (Security checked)
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            var currentUserName = User.Identity!.Name;
            if (User.IsInRole("Customer") && booking.UserId != currentUserName)
                return Forbid();

            return View(booking);
        }

        // =========================
        // CREATE (GET)
        // =========================
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateBookingVM
            {
                Flights = await GetFlightsDropdownAsync()
            };

            var currentUserName = User.Identity!.Name;
            var hasPassengers = await _context.Passengers.AnyAsync(p => p.UserId == currentUserName);

            if (!hasPassengers)
                TempData["Info"] = "You need to create a passenger first before booking.";

            return View(vm);
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Flights = await GetFlightsDropdownAsync();
                return View(vm);
            }

            var userId = User.Identity!.Name!;

            // ✅ Use transaction so seats + booking consistent
            await using var tx = await _context.Database.BeginTransactionAsync();

            // Load Flight (FOR UPDATE style not available; but transaction reduces issues)
            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == vm.FlightId);
            if (flight == null)
            {
                await tx.RollbackAsync();
                return NotFound("Flight details nahi milin.");
            }

            // ✅ Prevent booking on departed flights
            if (flight.DepartureTime <= DateTime.Now)
            {
                ModelState.AddModelError("", "You cannot book a flight that has already departed.");
                vm.Flights = await GetFlightsDropdownAsync();
                await tx.RollbackAsync();
                return View(vm);
            }

            // ✅ Seat availability check
            if (!HasEnoughSeats(flight, vm.SeatClass, vm.SeatCount))
            {
                ModelState.AddModelError("SeatCount", "Not enough seats available for this flight/class.");
                vm.Flights = await GetFlightsDropdownAsync();
                await tx.RollbackAsync();
                return View(vm);
            }

            // ✅ DUPLICATE BOOKING CHECK
            // Rule: same passport + same flight => block
            bool alreadyBooked = await _context.Bookings
                .Include(b => b.Passenger)
                .AnyAsync(b =>
                    b.FlightId == vm.FlightId &&
                    b.Passenger != null &&
                    b.Passenger.PassportNumber == vm.Passenger.PassportNumber);

            if (alreadyBooked)
            {
                ModelState.AddModelError("", "This passenger (same passport) is already booked on this flight.");
                vm.Flights = await GetFlightsDropdownAsync();
                await tx.RollbackAsync();
                return View(vm);
            }

            // ✅ Reuse passenger if same passport already exists for this user
            var passenger = await _context.Passengers.FirstOrDefaultAsync(p =>
                p.UserId == userId && p.PassportNumber == vm.Passenger.PassportNumber);

            if (passenger == null)
            {
                passenger = new Passenger
                {
                    First_Name = vm.Passenger.First_Name,
                    Last_Name = vm.Passenger.Last_Name,
                    PassportNumber = vm.Passenger.PassportNumber,
                    Age = vm.Passenger.Age,
                    Nationality = vm.Passenger.Nationality,
                    ContactEmail = vm.Passenger.ContactEmail,
                    PhoneNumber = vm.Passenger.PhoneNumber,
                    UserId = userId
                };

                _context.Passengers.Add(passenger);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Optional: keep data fresh (safe + helpful)
                passenger.First_Name = vm.Passenger.First_Name;
                passenger.Last_Name = vm.Passenger.Last_Name;
                passenger.Age = vm.Passenger.Age;
                passenger.Nationality = vm.Passenger.Nationality;
                passenger.ContactEmail = vm.Passenger.ContactEmail;
                passenger.PhoneNumber = vm.Passenger.PhoneNumber;
                _context.Passengers.Update(passenger);
                await _context.SaveChangesAsync();
            }

            // ✅ Price calc
            var basePrice = GetBasePrice(flight, vm.SeatClass);
            var multiplier = GetPassengerMultiplier(vm.PassengerType);
            var totalPrice = basePrice * multiplier * vm.SeatCount;

            // ✅ Deduct seats
            DeductSeats(flight, vm.SeatClass, vm.SeatCount);
            _context.Flights.Update(flight);

            // ✅ Create booking
            var booking = new Booking
            {
                FlightId = vm.FlightId,
                PassengerId = passenger.Id,
                SeatClass = vm.SeatClass,
                SeatCount = vm.SeatCount,
                PassengerType = vm.PassengerType,
                BookingDate = DateTime.Now,
                TotalPrice = totalPrice,
                BookingReference = await GenerateUniqueBookingReferenceAsync(),
                UserId = userId,
                IsPaid = false
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            // ✅ Email confirmation (after commit)
            await _emailService.SendBookingConfirmationAsync(
                passenger.ContactEmail,
                passenger.FullName,
                flight.FlightNumber,
                flight.Destination,
                flight.DepartureTime,
                booking.BookingReference,
                booking.TotalPrice
            );

            return RedirectToAction("Summary", new { id = booking.Id });
        }

        // =========================
        // EDIT (Admin only)
        // =========================
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

            await using var tx = await _context.Database.BeginTransactionAsync();

            var existing = await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == booking.Id);

            if (existing == null)
            {
                await tx.RollbackAsync();
                return NotFound();
            }

            // Do not allow editing to past flight time (optional safety)
            var targetFlight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == booking.FlightId);
            if (targetFlight == null)
            {
                await tx.RollbackAsync();
                return BadRequest();
            }

            // If changing seats/class/flight, restore old seats and deduct new seats
            bool seatOrClassOrFlightChanged =
                existing.FlightId != booking.FlightId ||
                existing.SeatClass != booking.SeatClass ||
                existing.SeatCount != booking.SeatCount;

            if (seatOrClassOrFlightChanged)
            {
                // Restore seats to OLD flight
                var oldFlight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == existing.FlightId);
                if (oldFlight == null)
                {
                    await tx.RollbackAsync();
                    return BadRequest();
                }

                RestoreSeats(oldFlight, existing.SeatClass, existing.SeatCount);
                _context.Flights.Update(oldFlight);

                // Deduct seats from NEW flight
                if (!HasEnoughSeats(targetFlight, booking.SeatClass, booking.SeatCount))
                {
                    ModelState.AddModelError("SeatCount", "Not enough seats available for the updated selection.");
                    PopulateDropdowns(booking);
                    await tx.RollbackAsync();
                    return View(booking);
                }

                DeductSeats(targetFlight, booking.SeatClass, booking.SeatCount);
                _context.Flights.Update(targetFlight);
            }

            // Recalculate price
            var basePrice = GetBasePrice(targetFlight, booking.SeatClass);
            var multiplier = GetPassengerMultiplier(booking.PassengerType);
            booking.TotalPrice = basePrice * multiplier * booking.SeatCount;

            // Update fields
            existing.FlightId = booking.FlightId;
            existing.PassengerId = booking.PassengerId;
            existing.SeatCount = booking.SeatCount;
            existing.SeatClass = booking.SeatClass;
            existing.PassengerType = booking.PassengerType;
            existing.IsPaid = booking.IsPaid;
            existing.TotalPrice = booking.TotalPrice;
            existing.BookingDate = booking.BookingDate;
            existing.UserId = booking.UserId;
            existing.BookingReference = booking.BookingReference;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // CANCEL (GET) Customer/Admin allowed with ownership checks
        // =========================
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            var currentUserName = User.Identity!.Name;
            if (User.IsInRole("Customer") && booking.UserId != currentUserName)
                return Forbid();

            return View(booking);
        }

        // =========================
        // CANCEL (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                await tx.RollbackAsync();
                return NotFound();
            }

            var currentUserName = User.Identity!.Name;
            if (User.IsInRole("Customer") && booking.UserId != currentUserName)
            {
                await tx.RollbackAsync();
                return Forbid();
            }

            if (booking.Flight != null && booking.Flight.DepartureTime <= DateTime.Now)
            {
                TempData["Error"] = "You cannot cancel a booking after the flight has departed.";
                await tx.RollbackAsync();
                return RedirectToAction(nameof(MyBookings));
            }

            // Restore seats
            if (booking.Flight != null)
            {
                RestoreSeats(booking.Flight, booking.SeatClass, booking.SeatCount);
                _context.Flights.Update(booking.Flight);
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            TempData["Success"] = "Booking cancelled successfully.";
            return RedirectToAction(nameof(MyBookings));
        }

        // =========================
        // DELETE (Admin only)
        // =========================
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
            await using var tx = await _context.Database.BeginTransactionAsync();

            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                await tx.RollbackAsync();
                return NotFound();
            }

            if (booking.Flight != null)
            {
                RestoreSeats(booking.Flight, booking.SeatClass, booking.SeatCount);
                _context.Flights.Update(booking.Flight);
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // ✅ OPTIONAL: Customer “Delete” (actually cancel) endpoint (no UI changes required)
        // If you later add a Delete button for customer, point it here.
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteMy(int id)
        {
            // Reuse Cancel view (same UI file)
            return await Cancel(id);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMyConfirmed(int id)
        {
            // Reuse CancelConfirmed logic (same behaviour)
            return await CancelConfirmed(id);
        }

        // =========================
        // MARK PAID (Admin only)
        // =========================
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

        // =========================
        // SUMMARY (Security checked)
        // =========================
        public async Task<IActionResult> Summary(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            var currentUserName = User.Identity!.Name;
            if (User.IsInRole("Customer") && booking.UserId != currentUserName)
                return Forbid();

            var vm = ToSummaryVm(booking);
            return View(vm);
        }

        // =========================
        // E-TICKET PDF (Security checked)
        // =========================
        [HttpGet]
        public async Task<IActionResult> ETicket(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            var currentUserName = User.Identity!.Name;
            if (User.IsInRole("Customer") && booking.UserId != currentUserName)
                return Forbid();

            var vm = ToSummaryVm(booking);

            var document = new ETicketDocument(vm);
            var pdfBytes = document.GeneratePdf();

            var fileName = $"ETicket_{vm.BookingReference}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        // =========================
        // PAYMENT SUCCESS (Customer only)
        // =========================
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentSuccess(int bookingId)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            var booking = await _context.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                await tx.RollbackAsync();
                return NotFound();
            }

            if (booking.UserId != User.Identity!.Name)
            {
                await tx.RollbackAsync();
                return Forbid();
            }

            // Prevent payment after departure
            if (booking.Flight != null && booking.Flight.DepartureTime <= DateTime.Now)
            {
                TempData["Error"] = "You cannot pay after flight departure.";
                await tx.RollbackAsync();
                return RedirectToAction(nameof(MyBookings));
            }

            // If already paid, just show success page (prevents double charge feeling)
            if (booking.IsPaid)
            {
                await tx.RollbackAsync();
                return View("PaymentSuccess");
            }

            booking.IsPaid = true;
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            // Generate PDF for attachment
            var vm = ToSummaryVm(booking);
            var document = new ETicketDocument(vm);
            var pdfBytes = document.GeneratePdf();
            var fileName = $"ETicket_{vm.BookingReference}.pdf";

            // Email + PDF attachment
            await _emailService.SendPaymentSuccessEmailWithPdfAsync(
                booking.Passenger!.ContactEmail,
                booking.Passenger.FullName,
                booking.Flight!.FlightNumber,
                booking.Flight.Origin,
                booking.Flight.Destination,
                booking.Flight.DepartureTime,
                booking.BookingReference,
                booking.TotalPrice,
                pdfBytes,
                fileName
            );

            // ✅ Use your existing PaymentSuccess.cshtml view (no UI changes)
            return View("PaymentSuccess");
        }
    }
}
