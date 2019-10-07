using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirplaneBookingSystem.Data;
using System.Linq;



namespace AirplaneBookingSystem.Controllers
{
    public class FlightController : Controller
    {
        private readonly UserContext ctx;

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
    }
}