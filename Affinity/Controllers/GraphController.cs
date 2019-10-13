using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Affinity.Models;
using System.Drawing;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Affinity.Controllers
{
    public class GraphController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private String API_KEY;

        public GraphController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            API_KEY = "";
            using (StreamReader reader = new StreamReader("API_KEY.txt"))
            {
                API_KEY = reader.ReadLine();
            }           
        }

        public String getDatabaseUrl()
        {
            if (API_KEY == "") return "error";
            RestClient client = new RestClient("https://api.heroku.com/");
            RestRequest req = new RestRequest("apps/affinity-cpp/config-vars");
            req.AddHeader("Accept", "application/vnd.heroku+json; version=3");
            req.AddHeader("Authorization", "Bearer " + API_KEY);
            String response = client.Execute(req).Content.ToLower();
            
            if(response.Contains("error"))
            {
                return "error";
            }
  
            JObject config = JObject.Parse(response);
            JProperty dbUrlProperty = config.Property("database_url");
            if(dbUrlProperty == null)
            {
                return "error";
            } else
            {
                return dbUrlProperty.Value.ToString();
            }
        }

        [Route("/dbtest")]
        public IActionResult dbTest()
        {
            return Json(new { url = getDatabaseUrl() });
        }

        [Route("/graph")]
        public async Task<string> Index()
        {
            Random rand = new Random();
            string tempID = "";
            int randomNum;
            while (tempID.Length < 8)
            {
                randomNum = rand.Next(128);
                if ((randomNum >= 49 && randomNum <= 57) || (randomNum >= 97 && randomNum <= 102))
                {
                    tempID += Convert.ToChar(randomNum);
                }
            }

            return tempID;
            /*return RedirectToRoute(new
            {
                Controller = "Graph",
                Action = "GetSpecificGraph",
                id = 0
            });*/   
        }

        [Route("/graph/{id}")]
        public async Task<IActionResult> GetSpecificGraph()
        {
            bool authenticated = await Utils.CheckFirebaseToken(_httpContextAccessor);
            if (!authenticated)
            {
                return RedirectToAction("Login","Affinity");
            }

            return Json(new { id = "1", value = "GetSpecificGraph" });
        }

        [Route("/graphs")]
        public async Task<IActionResult> GetGraphs()
        {
            bool authenticated = await Utils.CheckFirebaseToken(_httpContextAccessor);
            if (!authenticated)
            {
                return RedirectToAction("Login", "Affinity");
            }

            return Json(new { id = "2", value = "GetGraphs" });
        }

        [Route("/api/testneighbors")]
        public string TestGraphNeighbors()
        {
            Graph graph = new Graph();

            Vertex vertex = new Vertex
            {
                Name = "A",
                ID = 0,
                XPos = 200,
                YPos = 300,
                Color = Color.Aqua.ToArgb(),
            };

            Vertex vertex2 = new Vertex
            {
                Name = "B",
                ID = 1,
                XPos = 400,
                YPos = 300,
                Color = Color.Aqua.ToArgb()
            };

            Vertex vertex3 = new Vertex
            {
                Name = "C",
                ID = 2,
                XPos = 300,
                YPos = 200,
                Color = Color.Aqua.ToArgb()
            };

            Vertex vertex4 = new Vertex
            {
                Name = "D",
                ID = 3,
                XPos = 500,
                YPos = 400,
                Color = Color.Aqua.ToArgb()
            };

            graph.AddVertex(vertex);
            graph.AddVertex(vertex2);
            graph.AddVertex(vertex3);
            graph.AddVertex(vertex4);

            var Edges = new List<Edge>
            {
                new Edge
                {
                    Weight = 8,
                    First = 0,
                    Second = 3,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb()
                },
                new Edge
                {
                    Weight = 3,
                    First = 0,
                    Second = 2,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb()
                },
                new Edge
                {
                    Weight = 18,
                    First = 1,
                    Second = 2,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb()
                },
                new Edge
                {
                    Weight = 3,
                    First = 2,
                    Second = 0,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb()
                },
                new Edge
                {
                    Weight = 18,
                    First = 2,
                    Second = 1,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb()
                },
                new Edge
                {
                    Weight = 3,
                    First = 2,
                    Second = 3,
                    Direction = Direction.DirectedAtSecond,
                    Color = Color.Black.ToArgb()
                },
                new Edge
                {
                    Weight = 1,
                    First = 3,
                    Second = 1,
                    Direction = Direction.DirectedAtSecond,
                    Color = Color.Black.ToArgb()
                },
                new Edge
                {
                    Weight = 8,
                    First = 3,
                    Second = 0,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb()
                }
            };

            foreach (Edge edge in Edges)
            {
                graph.AddEdge(edge.First, edge.Second);
            }

            return JsonConvert.SerializeObject(graph.GetNeighbors(vertex), Formatting.Indented);
        }
    }
}
