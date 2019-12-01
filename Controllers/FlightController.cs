using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirplaneBookingSystem.Data;
using AirplaneBookingSystem.Models;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace AirplaneBookingSystem.Controllers
{
    public class FlightController : Controller
    {
        private readonly Db_Context ctx;

        public FlightController(Db_Context dbContext) {
            this.ctx = dbContext;         
        }

        int[] seatNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
            15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 
            28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 }; //our planes dont have more than 40 seats

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
                await ctx.SaveChangesAsync(); // Save changes, Needs to be saved in order to access the right flightid.
                for (int i=0;i<flight.FreeSeats;i++)
                {
                    FreeSeats FreeSeat = new FreeSeats //creates new free seat 
                    {
                        isFree = true,
                        SeatNumber = seatNumbers[i],
                        FlightId=flight.FlightId 
                    };
                    ctx.FreeSeats.Add(FreeSeat);
                    await ctx.SaveChangesAsync(); // Save changes
                }
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

            if (!User.Identity.IsAuthenticated)
                return View("Views/Errors/UserNotFound.cshtml");
            else if (HasJustReceivedMessage())
                ViewData["HasJustReceivedMessage"] = true;
            else
                ViewData["HasJustReceivedMessage"] = false;
            var currentUser = await ctx.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            currentUser.HasJustReceivedMessage = false;
            await ctx.SaveChangesAsync();

            return View(await ctx.Flights.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int flightId) {
            foreach(var currentUserFlight in ctx.UserFlights) //for each userflight in the database
            {
                User currentUser = await ctx.Users.FindAsync(currentUserFlight.UserId); //gets user
                Flight currentFlight = await ctx.Flights.FindAsync(currentUserFlight.FlightId);//gets flight
                if(HasJustReceivedMessage()==false)
                    currentUser.HasJustReceivedMessage = true; //since flight is being deleted allows the user to receive a messagr
                if(currentUserFlight.FlightId==flightId)//if this userflight contains the id of the flight the admin removed, erase the whole row.
                {
                    ctx.UserFlights.Remove(currentUserFlight);
                }
            }
            foreach(var currentFreeSeat in ctx.FreeSeats)
            {
                if(currentFreeSeat.FlightId == flightId)
                    ctx.FreeSeats.Remove(currentFreeSeat);
            }
            var currentF = await ctx.Flights.FindAsync(flightId);
            ctx.Flights.Remove(currentF);//after we delete everything in userflight we need to remove the flight from the flight list of course
            await ctx.SaveChangesAsync(); //saves changes to db.
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

        public IActionResult SeatSelect(int id)
        {
            List<FreeSeats> freeSeats = new List<FreeSeats>();
            foreach (var free in ctx.FreeSeats)
            {
                if (free.FlightId == id)
                {
                    freeSeats.Add(free);
                }
            }
            return View(freeSeats);
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

        public ViewResult SSS(int? id)
        {

            foreach (var a in ctx.FreeSeats)
            {
                string chosenSeatString = Request.Query["chosenSeat"];
                Debug.WriteLine(chosenSeatString);

                Debug.WriteLine(id);
                if (id != null && chosenSeatString!=null)
                {
                    if (a.FlightId == id && a.SeatNumber == Int32.Parse(chosenSeatString))
                    {
                        
                        a.isFree = false;
                        ctx.SaveChanges();
                    }
                }
            }
            return View();
        }
        private bool IsAdmin() {

            var currentUser =  ctx.Users.Find(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if (currentUser.IsAdmin)
                return true;

            return false;
        }

        private bool HasJustReceivedMessage()
        {
            var currentUser = ctx.Users.Find(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (currentUser.HasJustReceivedMessage)
                return true;

            return false;
        }

        private bool FlightExists(int id) {
            return ctx.Flights.Any(e => e.FlightId == id);
        }
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
                return NotFound();
            // get the current flight and current user
            var currentUser = await ctx.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var currentFlight = await ctx.Flights.FindAsync(id);
            // Create a User Flight so we know who had this flight for the View
            var userFlight = new UserFlights
            {
                User = currentUser,
                Flight = currentFlight
            };
            if (userFlight != null)
            {
                ctx.UserFlights.Remove(userFlight);  // removes the user flight from the db
                ++currentFlight.FreeSeats;        // assigns the seat as free
                await ctx.SaveChangesAsync();     // save changes to db
            }
            return View(userFlight);
        }
    }
}