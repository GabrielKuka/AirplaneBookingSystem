using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AirplaneBookingSystem.Models
{
    public class FreeSeats
    {
        public int FreeSeatsId { get; set; }
        public int SeatNumber { get; set; }

        public bool isFree { get; set; }
        [ForeignKey("FlightId")]
        public int FlightId { get; set; }
    }
}
