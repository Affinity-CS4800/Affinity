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
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
        public async Task<IActionResult> Index()
        {
            bool unique = true;
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

                var idList = _affinityDbContext.Users.AsNoTracking().Select(id => id.GraphID).Distinct();
                foreach(var id in idList)
                {
                    if(id == tempID)
                    {
                        unique = false;
                        break;
                    }
                    else
                    {
                        unique = true;
                    }
                }
            } while (!unique);

            //If user is logged in
            if (await Utils.CheckFirebaseToken(_httpContextAccessor))
            {
                //Get the token!
                var userToken = await Utils.GetUserFirebaseToken(_httpContextAccessor);
                int graphsMadeByUser = await _affinityDbContext.Users.AsNoTracking().Where(user => user.UID == userToken.Uid).CountAsync();

                //User has less than the max amount of graphs saved
                if (graphsMadeByUser < GraphConstants.MAX_GRAPHS)
                {
                    await _affinityDbContext.AddAsync(new User { UID = userToken.Uid, GraphID = tempID });
                    await _affinityDbContext.SaveChangesAsync();
                }
            }

            //_httpContextAccessor.HttpContext.Response.Cookies.Append("GraphID", tempID);

            //Debug.WriteLine(_httpContextAccessor.HttpContext.Request.Cookies["GraphID"]);

            return RedirectToAction("GetSpecificGraph", "Graph", new { token = tempID });
        }

        [Route("/graph/{token:length(8)}")]
        public async Task<IActionResult> GetSpecificGraph(string token)
        {
            bool authenticated = await Utils.CheckFirebaseToken(_httpContextAccessor);
            if (!authenticated)
            {
                return RedirectToAction("Login","Affinity");
            }
            //d9147ab4
            var graph = await _affinityDbContext.Vertices.AsNoTracking().Where(id => id.GraphID == token).ToListAsync();
            var graph2 = await _affinityDbContext.Edges.AsNoTracking().Where(id => id.GraphID == token).ToListAsync();

            return Content(JsonConvert.SerializeObject(graph) + "\n" + JsonConvert.SerializeObject(graph2));
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

        [Route("/api/testaddtodb")]
        public void TestAddToDB()
        {
            //Graph graph = new Graph();

            Vertex vertex1 = new Vertex
            {
                Name = "A",
                ID = 0,
                XPos = 200,
                YPos = 300,
                Color = Color.Aqua.ToArgb(),
                GraphID = "d9147ab4"
            };

            Vertex vertex2 = new Vertex
            {
                Name = "B",
                ID = 1,
                XPos = 400,
                YPos = 300,
                Color = Color.Aqua.ToArgb(),
                GraphID = "d9147ab4"
            };

            Vertex vertex3 = new Vertex
            {
                Name = "C",
                ID = 2,
                XPos = 300,
                YPos = 200,
                Color = Color.Aqua.ToArgb(),
                GraphID = "d9147ab4"
            };

            Vertex vertex4 = new Vertex
            {
                Name = "D",
                ID = 3,
                XPos = 500,
                YPos = 400,
                Color = Color.Aqua.ToArgb(),
                GraphID = "d9147ab4"
            };

            //graph.AddVertex(vertex1);
            //graph.AddVertex(vertex2);
            //graph.AddVertex(vertex3);
            //graph.AddVertex(vertex4);

            var Edges = new List<Edge>
            {
                new Edge
                {
                    Weight = 8,
                    First = 0,
                    Second = 3,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb(),
                    GraphID = "d9147ab4"
                },
                new Edge
                {
                    Weight = 3,
                    First = 0,
                    Second = 2,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb(),
                    GraphID = "d9147ab4"
                },
                new Edge
                {
                    Weight = 18,
                    First = 1,
                    Second = 2,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb(),
                    GraphID = "d9147ab4"
                },
                new Edge
                {
                    Weight = 3,
                    First = 2,
                    Second = 0,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb(),
                    GraphID = "d9147ab4"
                },
                new Edge
                {
                    Weight = 18,
                    First = 2,
                    Second = 1,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb(),
                    GraphID = "d9147ab4"
                },
                new Edge
                {
                    Weight = 3,
                    First = 2,
                    Second = 3,
                    Direction = Direction.DirectedAtSecond,
                    Color = Color.Black.ToArgb(),
                    GraphID = "d9147ab4"
                },
                new Edge
                {
                    Weight = 1,
                    First = 3,
                    Second = 1,
                    Direction = Direction.DirectedAtSecond,
                    Color = Color.Black.ToArgb(),
                    GraphID = "d9147ab4"
                },
                new Edge
                {
                    Weight = 8,
                    First = 3,
                    Second = 0,
                    Direction = Direction.Undirected,
                    Color = Color.Black.ToArgb(),
                    GraphID = "d9147ab4"
                }
            };


            _affinityDbContext.Vertices.Add(vertex1);
            _affinityDbContext.Vertices.Add(vertex2);
            _affinityDbContext.Vertices.Add(vertex3);
            _affinityDbContext.Vertices.Add(vertex4);

            foreach (Edge edge in Edges)
            {
                //graph.AddEdge(edge.First, edge.Second, "", edge.Color, edge.Weight, edge.Direction);
                _affinityDbContext.Edges.Add(edge);
            }

            _affinityDbContext.SaveChanges();

            //string output = "";

            //output += JsonConvert.SerializeObject(graph.GetNeighbors(vertex1), Formatting.Indented);

            //output += JsonConvert.SerializeObject(graph.Dijkstra(vertex1), Formatting.Indented);

            //output += $"\nGraph distance from {vertex1.ID} to {vertex2.ID} is: {graph.CalculateGraphDistance(vertex1, vertex2)}";
            //output += $"\nGraph distance from {vertex1.ID} to {vertex3.ID} is: {graph.CalculateGraphDistance(vertex1, vertex3)}";
            //output += $"\nGraph distance from {vertex1.ID} to {vertex4.ID} is: {graph.CalculateGraphDistance(vertex1, vertex4)}";

            //output += $"\nGraph Diameter is: {graph.CalculateGraphDiameter(graph)}";

            //return output;
        }
    }
}
