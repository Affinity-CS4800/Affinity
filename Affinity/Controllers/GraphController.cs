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

        [Route("/api/testGraph")]
        public string TestGraph()
        {
            Graph graph = new Graph();

            Vertex vertex1 = new Vertex { ID = 0, Name = "A", XPos = 200, YPos = 300, Color = Color.Blue.ToArgb() };
            Vertex vertex2 = new Vertex { ID = 1, Name = "B", XPos = 250, YPos = 250, Color = Color.Red.ToArgb() };
            Vertex vertex3 = new Vertex { ID = 2, Name = "C", XPos = 400, YPos = 250, Color = Color.Green.ToArgb() };


            graph.AddVertex(vertex1);
            graph.AddVertex(vertex2);
            graph.AddVertex(vertex3);

            graph.AddEdge(vertex1, vertex2);
            graph.AddEdge(vertex3, vertex1, "C to A", Color.White.ToArgb(), 5, Direction.DirectedAtFirst);

            return graph.PrintAdjacencyList();
        }
    }
}
