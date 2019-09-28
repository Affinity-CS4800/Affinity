using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Affinity.Models;
using System.Drawing;

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

        [Route("/api/test")]
        public string TestNewtonSoft()
        {
            Vertex vertex = new Vertex();

            vertex.Name = "A";
            vertex.XPos = 200;
            vertex.YPos = 300;
            vertex.Color = Color.White.ToArgb();
            vertex.Edges = new List<Edge>
            {
                new Edge
                {
                    ID = 1,
                    Weight = 8,
                    First = 1,
                    Second = 2,
                    Color = Color.Black.ToArgb()
                },
                new Edge
                {
                    ID = 2,
                    Weight = 3,
                    First = 2,
                    Second = 1,
                    Color = Color.Black.ToArgb()
                }
            };

            return JsonConvert.SerializeObject(vertex, Formatting.Indented);
        }
    }
}
