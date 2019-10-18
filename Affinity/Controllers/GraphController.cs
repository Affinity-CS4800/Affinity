﻿using System;
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
using Microsoft.EntityFrameworkCore;

namespace Affinity.Controllers
{
    public class GraphController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AffinityDbcontext _affinityDbContext;
        private string API_KEY;

        public GraphController(IHttpContextAccessor httpContextAccessor, AffinityDbcontext affinityDbcontext)
        {
            _httpContextAccessor = httpContextAccessor;
            _affinityDbContext = affinityDbcontext;
            API_KEY = "";
            using (StreamReader reader = new StreamReader("API_KEY.txt"))
            {
                API_KEY = reader.ReadLine();
            }
        }

        public string GetDatabaseUrl()
        {
            if (API_KEY == "")
            {
                return "error";
            }

            RestClient client = new RestClient("https://api.heroku.com/");
            RestRequest req = new RestRequest("apps/affinity-cpp/config-vars");
            req.AddHeader("Accept", "application/vnd.heroku+json; version=3");
            req.AddHeader("Authorization", "Bearer " + API_KEY);
            string response = client.Execute(req).Content.ToLower();
            
            if(response.Contains("error"))
            {
                return "error";
            }
  
            JObject config = JObject.Parse(response);
            JProperty dbUrlProperty = config.Property("database_url");

            if(dbUrlProperty == null)
            {
                return "error";
            }
            
            return dbUrlProperty.Value.ToString();
        }

        [Route("/dbtest")]
        public IActionResult DbTest()
        {
            return Json(new { url = GetDatabaseUrl() });
        }

        [Route("/graph")]
        public async Task<string> Index()
        {
            bool unique = false;
            Random rand = new Random();
            string tempID;
            int randomNum;
            do
            {
                tempID = "";
                while (tempID.Length < 8)
                {
                    randomNum = rand.Next(53) + 49;
                    if ((randomNum >= 49 && randomNum <= 57) || (randomNum >= 97 && randomNum <= 102))
                    {
                        tempID += Convert.ToChar(randomNum);
                    }
                }

                var idList = _affinityDbContext.Users.AsNoTracking().Include(g => g.GraphIds).ToList();
                foreach(var i in idList)
                {
                    if (i == tempID)
                    {
                        unique = false;
                        break;
                    }
                    else
                        unique = true;
                }


            } while (!unique);

            if ((await Utils.CheckFirebaseToken(_httpContextAccessor)))
            {
                var user = await Utils.GetUserFirbaseToken(_httpContextAccessor);
                var count = await _affinityDbContext.Users.Where(u => u.UId == user.Uid).CountAsync();

                if (count == 0)
                {
                    _affinityDbContext.Users.Add(new User { UId = user.Uid, GraphIds = new List<GraphID>() });
                }
                _affinityDbContext.Users.Add(new GraphID { graphID = tempID });


            }

            return RedirectToRoute(new
            {
                Controller = "Graph",
                Action = "GetSpecificGraph",
                id = tempID
            }); 
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

        [Route("/api/testgraph")]
        public string TestGraphNeighbors()
        {
            Graph graph = new Graph();

            Vertex vertex1 = new Vertex
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

            graph.AddVertex(vertex1);
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
                graph.AddEdge(edge.First, edge.Second, "", edge.Color, edge.Weight, edge.Direction);
            }

            string output = "";

            output += JsonConvert.SerializeObject(graph.GetNeighbors(vertex1), Formatting.Indented);

            output += JsonConvert.SerializeObject(graph.Dijkstra(vertex1), Formatting.Indented);

            output += $"\nGraph distance from {vertex1.ID} to {vertex2.ID} is: {graph.CalculateGraphDistance(vertex1, vertex2)}";
            output += $"\nGraph distance from {vertex1.ID} to {vertex3.ID} is: {graph.CalculateGraphDistance(vertex1, vertex3)}";
            output += $"\nGraph distance from {vertex1.ID} to {vertex4.ID} is: {graph.CalculateGraphDistance(vertex1, vertex4)}";

            output += $"\nGraph Diameter is: {graph.CalculateGraphDiameter(graph)}";

            return output;
        }
    }
}
