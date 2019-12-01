using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AirplaneBookingSystem.Models;
using AirplaneBookingSystem.Data;
using System.Security.Claims;

namespace AirplaneBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly Db_Context ctx;
        public HomeController(ILogger<HomeController> logger, Db_Context dbcontext)
        {
            _logger = logger;
            this.ctx = dbcontext;
        }

        public async Task<IActionResult> IndexAsync()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
