using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace AirplaneBookingSystem.Models
{
    public class Flight
    {
        public int FlightId { get; set; }

        [DisplayName("Flight Number")]
        [Required]
        public string FlightNumber { get; set; }

        [Required]
        public string Departure { get; set; }

        [Required]
        public string Arrival { get; set; }

        [DisplayName("Departure Time")]
        [Required]
        public string DepartureTime { get; set; }

        [DisplayName("Arrival Time")]
        [Required]
        public string ArrivalTime { get; set; }

        [DisplayName("Number of Seats")]
        [Required]
        public int FreeSeats { get; set; }

        public IList<UserFlights> UserFlights { get; set; }

    }
}
