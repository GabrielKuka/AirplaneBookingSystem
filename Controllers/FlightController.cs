using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirplaneBookingSystem.Data;
using AirplaneBookingSystem.Models;
using System.Security.Claims;

namespace AirplaneBookingSystem.Controllers
{
    public class FlightController : Controller
    {
        private readonly Db_Context ctx;
        private readonly string _userId;
      

        public FlightController(Data.Db_Context userContext) {
            this.ctx = userContext;
            _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public  async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return View("Views/Errors/UserNotFound.cshtml");

            return View( await ctx.Flights.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Book(int? id) {

            if (id == null)
                return NotFound();

            
            // get the current flight and current user
            var currentUser = await ctx.Users.FindAsync(_userId);
            var currentFlight = await ctx.Flights.FindAsync(id);

            var userFlight = new UserFlights {             
                User = currentUser,         
                Flight = currentFlight
            };

            if (userFlight != null)
            {              
                ctx.UserFlights.Add(userFlight);  // add the user flight
                --currentFlight.FreeSeats;        // assign the seat as reserved
                await ctx.SaveChangesAsync();                // save changes to db
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

            // check if this flight is booked by this user
            foreach (var usrFlight in ctx.UserFlights) {
                if (usrFlight.FlightId == flight.FlightId && usrFlight.UserId == _userId)
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
    }
}