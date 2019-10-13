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

        [DataType(DataType.Date)]
        public string DepartureTime { get; set; }

        [DataType(DataType.Date)]
        public string ArrivalTime { get; set; }

        public int SeatsLeft { get; set; }


    }
}
