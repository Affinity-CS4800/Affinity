using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Affinity.Controllers
{
    public class GraphController : Controller
    {
        [Route("/graph")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/graph/{id}")]
        public IActionResult GetSpecificGraph()
        {
            return Json(new { id = "1", value = "GetSpecificGraph" });
        }

        [Route("/graphs")]
        public IActionResult GetGraphs()
        {
            return Json(new { id = "2", value = "GetGraphs" });
        }
    }
}
