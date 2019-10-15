using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace AirplaneBookingSystem.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IList<UserFlights> UserFlights { get; set; }
    }
}
