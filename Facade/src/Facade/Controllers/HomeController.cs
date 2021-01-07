using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Facade.Models;
using Facade.Services;

namespace Facade.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FacadeSession _facadeSession;

        public HomeController(ILogger<HomeController> logger, FacadeSession facadeSession)
        {
            _logger = logger;
            _facadeSession = facadeSession;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("hack")]
        public async Task<string> GetHack()
        {
            var cartId = await _facadeSession.GetAsync<string>("CartId");
            return cartId;
        }
    }
}
