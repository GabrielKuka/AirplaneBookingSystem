using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirplaneBookingSystem.Data;
using AirplaneBookingSystem.Models;
using System.Security.Claims;
using System.Linq;
<<<<<<< HEAD
using System.Collections.Generic;
using System;
using System.Diagnostics;
=======
using System;
using System.Collections.Generic;
>>>>>>> 523f9007df9922d6788d8dacf14d5d594724e7e4

namespace AirplaneBookingSystem.Controllers
{
    public class FlightController : Controller
    {
        private readonly Db_Context ctx;

        public FlightController(Db_Context dbContext) {
            this.ctx = dbContext;         
        }

<<<<<<< HEAD
        int[] seatNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
            15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 
            28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 }; //our planes dont have more than 40 seats

=======
        [HttpPost]
        public async Task<IActionResult> MoveToNextFlight(int id)
        {
            var currentFlight = await ctx.Flights.FindAsync(id);
            Flight nextFlight = null;

            // Check if there is a flight with the same dep and dest + a later departure date
            foreach (var flight in ctx.Flights) {
                if (flight.Departure == currentFlight.Departure && flight.Arrival == currentFlight.Arrival && flight.DepartureTime > currentFlight.DepartureTime)
                {
                    nextFlight = flight;
                    break;
                }
            }

            if (nextFlight != null) {
                // check if the next flight is already overbooked
                if (nextFlight.FreeSeats < 0 || nextFlight.FreeSeats < ctx.GetOverbookedUsersFromFlight(currentFlight).Count)
                    return View("/Views/Errors/NextFlightOverbookedError.cshtml");
                else
                { // if not, move the overbooked users there

                    // list of overbooked users from this flight
                    var overbookedUsers = ctx.GetOverbookedUsersFromFlight(currentFlight); 

                    // Removes all overbooked users and replaces them to the next available flight
                    for (int i = 0; i < overbookedUsers.Count; i++)
                    {   
                        ctx.UserFlights.Add(new UserFlights { User = ctx.GetUserFromEmail(overbookedUsers.ElementAt(i).Email), Flight = nextFlight });
                        --nextFlight.FreeSeats;

                        ctx.OverbookedUsers.Remove(overbookedUsers.ElementAt(i));
                        ctx.UserFlights.Remove(ctx.GetSpecificUserFlight(currentFlight, ctx.GetUserFromEmail(overbookedUsers.ElementAt(i).Email)));
                        ++currentFlight.FreeSeats;
                        await ctx.SaveChangesAsync();
                    }

                    return View("/Views/Success/MovedToNextFlight.cshtml");
                }
            }
            else
            {
                return View("/Views/Errors/NoNextFlightError.cshtml");
            }

          
        }
  
>>>>>>> 523f9007df9922d6788d8dacf14d5d594724e7e4
        [HttpGet]
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated) {
                return View("Views/Errors/MustBeLoggedIn.cshtml");
            }else if (IsAdmin()) {
                return View();
            } else
            {
                return View("Views/Errors/MustBeAdmin.cshtml");
            }
            
        }

        // Creates a flight (you need to be admin)
        [HttpPost]
       public async Task<IActionResult> Create([Bind("FlightId, FlightNumber, Departure, Arrival, DepartureTime, ArrivalTime, TotalSeats")] Flight flight)
        {
            
                if (ModelState.IsValid && IsTimeValid(flight.DepartureTime, flight.ArrivalTime))
                {               // Check validation 
                    
                
                ctx.Flights.Add(flight);            // Add the flight
<<<<<<< HEAD
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
=======
                flight.FreeSeats = flight.TotalSeats;

                await ctx.SaveChangesAsync();       // Save changes
                    return RedirectToAction("index", "flight");
                }

                return View(flight);
            
>>>>>>> 523f9007df9922d6788d8dacf14d5d594724e7e4

        }

        [HttpGet]
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
<<<<<<< HEAD
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
=======

                var currentFlight = await ctx.Flights.FindAsync(flightId);
                ctx.Flights.Remove(currentFlight);
                await ctx.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
                    
>>>>>>> 523f9007df9922d6788d8dacf14d5d594724e7e4
        }

      
        [HttpGet]
        public async Task<IActionResult> Edit(int? id) {
            if (!User.Identity.IsAuthenticated) {
                return View("Views/Errors/MustBeLoggedIn.cshtml");
            } else if (IsAdmin()) {

                if (id == null)
                    return NotFound();

                var currentFlight = await ctx.Flights.FindAsync(id);

                if (currentFlight == null)
                    return NotFound();

                return View(currentFlight);

            } else
            {
                return View("Views/Errors/MustBeAdmin.cshtml");
            }
            
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FlightId, FlightNumber, Departure, Arrival, DepartureTime, ArrivalTime, TotalSeats")] Flight currentFlight) {

            if (id != currentFlight.FlightId)
                return NotFound();

            

            if (ModelState.IsValid && IsTimeValid(currentFlight.DepartureTime, currentFlight.ArrivalTime)) {
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

                // Set the user as overbooked
                if (currentFlight.FreeSeats < 0) {
                    var overbookedUser = new OverbookedUser { Flight = currentFlight, Email = currentUser.Email };
                    ctx.OverbookedUsers.Add(overbookedUser);
                }

              
                await ctx.SaveChangesAsync();     // save changes to db
            }
                

            return View(userFlight);
        }

<<<<<<< HEAD
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
=======
>>>>>>> 523f9007df9922d6788d8dacf14d5d594724e7e4

        [HttpGet]
        public async Task<IActionResult> Details(int? id) {

            if (id == null) {
                return NotFound();
            }

            // get current flight
            var flight = await ctx.Flights.FindAsync(id);
            ViewData["isBooked"] = false;
            if (IsAdmin())
            {
                ViewData["isAdmin"] = true;
                List<String> overbookedUserEmails = new List<String>();
                foreach (var overbooked in ctx.OverbookedUsers) {
                    if (overbooked.Flight == flight)
                        overbookedUserEmails.Add(overbooked.Email);
                        
                }
                ViewData["OverBookedUsers"] = overbookedUserEmails;
            }
            else
            {
                ViewData["isAdmin"] = false;
            }

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

<<<<<<< HEAD
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
=======
        private bool IsTimeValid(DateTime departure, DateTime arrival)
        {
            return departure <= arrival && departure > DateTime.Now;
        }

>>>>>>> 523f9007df9922d6788d8dacf14d5d594724e7e4
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

        public async Task<IActionResult> CancelBooking(int? id)
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