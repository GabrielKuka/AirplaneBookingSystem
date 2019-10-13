using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirplaneBookingSystem.Data;
using System.Linq;
using WebMatrix.Data;
using AirplaneBookingSystem.Models;
using AirplaneBookingSystem.ViewModel;
using System.Collections.Generic;
using System.Security.Claims;

namespace AirplaneBookingSystem.Controllers
{
    public class FlightController : Controller
    {
        private UserContext ctx;

        public FlightController(UserContext userContext) {
            this.ctx = userContext;
        }

        public  async Task<IActionResult> Index()
        {
            return View( await ctx.Flights.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id) {

            if (id == null) {
                return NotFound();
            }

            var flight = await ctx.Flights.FindAsync(id);

            if (flight == null) {
                return NotFound();
            }

            return View(flight);
        }
        public async Task<IActionResult> SuccessfulAsync(int? id)
        {
            if (ctx.Flights.Find(id).SeatsLeft>0)
            {
                await ctx.UserFlights.AddAsync(new UserFlights { FlightId = (int)id, UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
                ctx.Flights.Find(id).SeatsLeft -= 1;
                ctx.SaveChanges();
            }
            else
            {
                return View("NotSuccessfulNoSeats");
            }
            return View();
        }
    }
}