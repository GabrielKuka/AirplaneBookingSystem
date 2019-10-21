using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace AirplaneBookingSystem.Models
{
    public class Flight
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }

        public string Departure { get; set; }

        public string Arrival { get; set; }

        public string DepartureTime { get; set; }

        public string ArrivalTime { get; set; }

        public int FreeSeats { get; set; }

        public IList<UserFlights> UserFlights { get; set; }

    }
}
