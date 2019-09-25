using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Affinity.Models;
using Microsoft.Extensions.Logging;

namespace Affinity.Controllers
{
    public class AffinityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("/about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("/login")]
        public IActionResult Login()
        {
            return View();
        }
        [Route("/register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("/api/testjson")]
        public IActionResult TestJson()
        {
            return Json(new { id = 1, value = "hello" });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        [Route("/api/logging-test")]
        public string TestLogging()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            return loggerFactory.ToString();
        }
    }
}
