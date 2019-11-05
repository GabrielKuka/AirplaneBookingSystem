using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirplaneBookingSystem.Data;
using AirplaneBookingSystem.Models;
using System.Security.Claims;
using System.Linq;

namespace AirplaneBookingSystem.Controllers
{
    public class FlightController : Controller
    {
        private readonly Db_Context ctx;

        public FlightController(Db_Context dbContext) {
            this.ctx = dbContext;         
        }


        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Creates a flight (you need to be admin)
        [HttpPost]
       public async Task<IActionResult> Create([Bind("FlightId, FlightNumber, Departure, Arrival, DepartureTime, ArrivalTime, FreeSeats")] Flight flight)
        {

            if (ModelState.IsValid) {               // Check validation 
                ctx.Flights.Add(flight);            // Add the flight
                await ctx.SaveChangesAsync();       // Save changes
                return RedirectToAction("index", "flight");
            }

            return View(flight);
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return View("Views/Errors/UserNotFound.cshtml");
            else if (IsAdmin())
                ViewData["isAdmin"] = true;
            else
                ViewData["isAdmin"] = false;

            return View(await ctx.Flights.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int flightId) {

            var currentFlight = await ctx.Flights.FindAsync(flightId);
            ctx.Flights.Remove(currentFlight);
            await ctx.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id) {
            if (id == null)
                return NotFound();

            var currentFlight = await ctx.Flights.FindAsync(id);

            if (currentFlight == null)
                return NotFound();
            
            return View(currentFlight);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FlightId, FlightNumber, Departure, Arrival, DepartureTime, ArrivalTime, FreeSeats")] Flight currentFlight) {

            if (id != currentFlight.FlightId)
                return NotFound();

            

            if (ModelState.IsValid) {
                try {
                    ctx.Update(currentFlight);
                    await ctx.SaveChangesAsync();

                } catch (DbUpdateConcurrencyException) {
                    if (FlightExists(id))
                        throw;
                    else
                        return NotFound();
                }

                if (IsAdmin())
                    ViewData["isAdmin"] = true;
                else
                    ViewData["isAdmin"] = false;

                return RedirectToAction(nameof(Index));
            }

            return View(currentFlight);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int? id) {

            if (id == null)
                return NotFound();

            
            // get the current flight and current user
            var currentUser = await ctx.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var currentFlight = await ctx.Flights.FindAsync(id);
          
            // Create the Booking
            var userFlight = new UserFlights {             
                User = currentUser,         
                Flight = currentFlight
            };

            if (userFlight != null)
            {              
                ctx.UserFlights.Add(userFlight);  // add the user flight to the db
                --currentFlight.FreeSeats;        // assign the seat as reserved
                await ctx.SaveChangesAsync();     // save changes to db
            }
                

            return View(userFlight);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id) {

            if (id == null) {
                return NotFound();
            }

            // get current flight
            var flight = await ctx.Flights.FindAsync(id);
            ViewData["isBooked"] = false;
            if (IsAdmin())
                ViewData["isAdmin"] = true;
            else
                ViewData["isAdmin"] = false;

            // check if this flight is booked by this user
            foreach (var usrFlight in ctx.UserFlights) {
                if (usrFlight.FlightId == flight.FlightId && usrFlight.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    ViewData["isBooked"] = true;
                    break;
                }

                
            }

            if (flight == null) {
                return NotFound();
            }

            return View(flight);
        }

        private bool IsAdmin() {

            var currentUser =  ctx.Users.Find(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if (currentUser.IsAdmin)
                return true;

            return false;
        }

        private bool FlightExists(int id) {
            return ctx.Flights.Any(e => e.FlightId == id);
        }
    }
}