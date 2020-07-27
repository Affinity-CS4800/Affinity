using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Affinity.Models;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Affinity.Controllers
{
    public class GraphController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AffinityDbcontext _affinityDbContext;

        public GraphController(IHttpContextAccessor httpContextAccessor, AffinityDbcontext affinityDbcontext)
        {
            _httpContextAccessor = httpContextAccessor;
            _affinityDbContext = affinityDbcontext;
        }

        [Route("/api/saveToDB/{graphID:length(8)}")]
        [HttpPost]
        public async Task GraphSaver([FromBody] JArray json, string graphID)
        {
            var userToken = await Utils.GetUserFirebaseToken(_httpContextAccessor);

            //If the graph doesnt exist for the user verify they are logged in and save to DB
            if(!await GraphExistForUser(userToken.Uid, graphID))
            {
                //Make and store the new graph into the database
                await CheckIfLoggedInAndMakeGraph(graphID);
            }

            var user = await _affinityDbContext.Users.Where(u => u.UID == userToken.Uid && u.GraphID == graphID).FirstOrDefaultAsync();
            user.Modified = DateTime.Now;
            _affinityDbContext.Users.Update(user);

            foreach (var key in json)
            {
                Debug.WriteLine(key);
                string value = key["action"].ToString();
                if (value == "0")
                {
                    Vertex vertex = new Vertex
                    {
                        Name = "",
                        ID = int.Parse(key["id"].ToString()),
                        Color = ColorTranslator.FromHtml(key["color"].ToString()).ToArgb(),
                        XPos = (int)double.Parse(key["x"].ToString()),
                        YPos = (int)double.Parse(key["y"].ToString()),
                        GraphID = graphID
                    };
                        
                    _affinityDbContext.Vertices.Add(vertex);
                }
                else if(value == "1")
                {
                    Edge edge = new Edge
                    {
                        ID = int.Parse(key["id"].ToString()),
                        First = int.Parse(key["from"].ToString()),
                        Second = int.Parse(key["to"].ToString()),
                        Direction = Convert.ToInt32(bool.Parse(key["isDirected"].ToString())),
                        Color = ColorTranslator.FromHtml(key["color"].ToString()).ToArgb(),
                        GraphID = graphID
                    };

                    _affinityDbContext.Edges.Add(edge);
                }
                else if(value == "2")
                {
                    Vertex vertex = await _affinityDbContext.Vertices.Where(v => v.ID == int.Parse(key["node"]["id"].ToString()) && v.GraphID == graphID).FirstOrDefaultAsync();
                    _affinityDbContext.Vertices.Remove(vertex);
                }
                else if(value == "3")
                {
                    var edges = _affinityDbContext.Edges.AsNoTracking().Where(v => (v.First == int.Parse(key["node"]["id"].ToString()) || v.Second == int.Parse(key["node"]["id"].ToString())) && v.GraphID == graphID)
                            .Select(g => g.DBID).Distinct();
                    foreach (var edge in edges)
                    {
                        Edge e = await _affinityDbContext.Edges.Where(v => v.DBID == edge && v.GraphID == graphID).FirstOrDefaultAsync();
                        _affinityDbContext.Edges.Remove(e);
                    }
                    Vertex vertex = await _affinityDbContext.Vertices.Where(v => v.ID == int.Parse(key["node"]["id"].ToString()) && v.GraphID == graphID).FirstOrDefaultAsync();
                    _affinityDbContext.Vertices.Remove(vertex);
                }
                else if(value == "4")
                {
                    Edge edge = await _affinityDbContext.Edges.Where(v => v.ID == int.Parse(key["edge"]["id"].ToString()) && v.GraphID == graphID).FirstOrDefaultAsync();
                    _affinityDbContext.Edges.Remove(edge);
                }
                else if(value == "5")
                {
                    Vertex vertex = await _affinityDbContext.Vertices.Where(v => v.ID == int.Parse(key["id"].ToString()) && v.GraphID == graphID).FirstOrDefaultAsync();
                    vertex.Color = ColorTranslator.FromHtml(key["color"].ToString()).ToArgb();
                    _affinityDbContext.Vertices.Update(vertex);
                }
                else if (value == "6")
                {
                    Vertex vertex = await _affinityDbContext.Vertices.Where(v => v.ID == int.Parse(key["id"].ToString()) && v.GraphID == graphID).FirstOrDefaultAsync();
                    vertex.Name = key["label"].ToString();
                    vertex.FontColor = key["fontColor"].ToString();
                    _affinityDbContext.Vertices.Update(vertex);
                }
                else if(value == "7")
                {
                    Vertex vertex = await _affinityDbContext.Vertices.Where(v => v.ID == int.Parse(key["id"].ToString()) && v.GraphID == graphID).FirstOrDefaultAsync();
                    vertex.FontColor = key["fontColor"].ToString();
                }
                else if (value == "8")
                {
                    Vertex vertex = await _affinityDbContext.Vertices.Where(v => v.ID == int.Parse(key["id"].ToString()) && v.GraphID == graphID).FirstOrDefaultAsync();
                    vertex.XPos = (int)double.Parse(key["x"].ToString());
                    vertex.YPos = (int)double.Parse(key["y"].ToString());
                    _affinityDbContext.Vertices.Update(vertex);
                }
                else if (value == "9")
                {
                    Edge edge = await _affinityDbContext.Edges.Where(v => v.ID == int.Parse(key["id"].ToString()) && v.GraphID == graphID).FirstOrDefaultAsync();
                    edge.Name = key["label"].ToString();
                    _affinityDbContext.Edges.Update(edge);
                }
                else if (value == "10")
                {
                    Edge edge = await _affinityDbContext.Edges.Where(v => v.ID == int.Parse(key["id"].ToString()) && v.GraphID == graphID).FirstOrDefaultAsync();
                    edge.Color = ColorTranslator.FromHtml(key["color"].ToString()).ToArgb();
                    _affinityDbContext.Edges.Update(edge);
                }
                else if(value == "11")
                {
                    Edge edge = await _affinityDbContext.Edges.Where(v => v.ID == int.Parse(key["id"].ToString()) && v.GraphID == graphID).FirstOrDefaultAsync();
                    edge.FontAlignment = key["alignment"].ToString();
                    _affinityDbContext.Edges.Update(edge);
                }
                else if(value == "12")
                {
                    Edge edge = await _affinityDbContext.Edges.Where(v => v.ID == int.Parse(key["id"].ToString()) && v.GraphID == graphID).FirstOrDefaultAsync();
                    edge.Direction = Convert.ToInt32(bool.Parse(key["ifDirected"].ToString()));
                    _affinityDbContext.Edges.Update(edge);
                }
                else if(value == "13")
                {
                    user.Name = key["graphName"].ToString();
                    _affinityDbContext.Users.Update(user);
                }
                
                await _affinityDbContext.SaveChangesAsync();
            }
        }

        [Route("/graph")]
        public IActionResult Index()
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

            return RedirectToAction("GetSpecificGraph", "Graph", new { token = tempID });
        }

        [Route("/graph/{token:length(8)}")]
        public async Task<IActionResult> GetSpecificGraph(string token)
        {
            //bool authenticated = await Utils.CheckFirebaseToken(_httpContextAccessor);
            //if (!authenticated)
            //{
            //    return RedirectToAction("Login","Affinity");
            //}

            var userToken = await Utils.GetUserFirebaseToken(_httpContextAccessor);
            var user = await _affinityDbContext.Users.AsNoTracking().Where(graph => graph.GraphID == token).FirstOrDefaultAsync();

            //If the database found no graph id associated with the token or the user's UID does not match the one logged in then boot em to their graphs
            if (user != null && userToken != null && user.UID != userToken.Uid)
            {
                return RedirectToAction("GetGraphs", "Graph");
            }

            return View(nameof(Index));
        }

        [Route("/graphs")]
        public async Task<IActionResult> GetGraphs()
        {
            bool authenticated = await Utils.CheckFirebaseToken(_httpContextAccessor);
            if (!authenticated)
            {
                return RedirectToAction("Login", "Affinity");
            }

            var userToken = await Utils.GetUserFirebaseToken(_httpContextAccessor);
            var graph = _affinityDbContext.Users.AsNoTracking().Where(user => user.UID == userToken.Uid).OrderByDescending(d => d.Modified);

            List<GraphIDName> graphs = new List<GraphIDName>();

            foreach(var user in graph)
            {
                graphs.Add(new GraphIDName { Name = user.Name, GraphID = user.GraphID });
            }

            ViewData["MaxGraphs"] = GraphConstants.MAX_GRAPHS;

            return View("Graphs", graphs);
        }

        [Route("/api/graphData/{token:length(8)}")]
        public async Task<IActionResult> GetGraphData(string token)
        {
            bool authenticated = await Utils.CheckFirebaseToken(_httpContextAccessor);
            //This will be triggered if the user is not logged in so this graph
            //will not be retreived from the backend database.
            if (!authenticated)
            {
                return Json(new { nonAuthUser = true });
            }

            //Get the current Firebase token and we know that they are logged in so there has to be a token for us to read
            var userToken = await Utils.GetUserFirebaseToken(_httpContextAccessor);
            //Get the graph owner of the current request. If its bad let the frontend know where to redirect to!
            var graphOwner = await _affinityDbContext.Users.Where(user => user.UID == userToken.Uid && user.GraphID == token).FirstOrDefaultAsync();

            //graph wasn't owned by current user redirect them to their graphs
            //if(graphOwner == null)
            //{
            //    return Json(new { redirect = true, redirect_url = "/graphs" });
            //}

            //d9147ab4
            //879443a4
            var vertices = await _affinityDbContext.Vertices.AsNoTracking().Where(id => id.GraphID == token).ToListAsync();
            var edges = await _affinityDbContext.Edges.AsNoTracking().Where(id => id.GraphID == token).ToListAsync();

            GraphDataJson graphData = new GraphDataJson
            {
                Vertices = vertices,
                Edges = edges
            };

            return Content(JsonConvert.SerializeObject(graphData));
        }

        [Route("/api/getStartingNodeID/{token:length(8)}")]
        public async Task<int> GetHighestNodeID(string token)
        {
            int? nodeID = await _affinityDbContext.Vertices.AsNoTracking().Where(id => id.GraphID == token).Select(i => i.ID).OrderByDescending(id => id).FirstOrDefaultAsync();

            if(nodeID > 0)
            {
                return nodeID.Value + 1;
            }

            return 0;
        }

        [Route("/api/getStartingEdgeID/{token:length(8)}")]
        public async Task<int> GetHighestEdgeID(string token)
        {
            int? edgeID = await _affinityDbContext.Edges.AsNoTracking().Where(id => id.GraphID == token).Select(i => i.ID).OrderByDescending(id => id).FirstOrDefaultAsync();

            if (edgeID > 0)
            {
                return edgeID.Value + 1;
            }

            return 0;
        }

        [Route("/api/getGraphName/{token:length(8)}")]
        public async Task<string> GetGraphName(string token)
        {
            //Get the current Firebase token and we know that they are logged in so there has to be a token for us to read
            var userToken = await Utils.GetUserFirebaseToken(_httpContextAccessor);

            string graphName = await _affinityDbContext.Users.AsNoTracking().Where(user => user.GraphID == token && user.UID == userToken.Uid).Select(name => name.Name).FirstOrDefaultAsync();

            return graphName;
        }

        [Route("/api/canUserSave/{token:length(8)}")]
        public async Task<bool> CheckIfUserCanSave(string token)
        {
            // Get the current Firebase token and we know that they are logged in so there has to be a token for us to read
            var userToken = await Utils.GetUserFirebaseToken(_httpContextAccessor);

            var graphs = await _affinityDbContext.Users.AsNoTracking().Where(user => user.UID == userToken.Uid).ToListAsync();

            //Check to make sure its id is in the previously saved GraphIDs or not
            if(graphs.Any(graph => graph.GraphID == token))
            {
                return true; //overwriting a previously saved graph so this is allowed
            }

            //Since a user doesn't have max saved we can now asses based on the amount of graphs detected for the user
            return GraphConstants.MAX_GRAPHS - graphs.Count > 0;
        }

        [Route("/api/getGraphNames")]
        public async Task<List<string>> GetGraphNames()
        {
            // Get the current Firebase token and we know that they are logged in so there has to be a token for us to read
            var userToken = await Utils.GetUserFirebaseToken(_httpContextAccessor);

            List<User> graphsTiedToUser = await _affinityDbContext.Users.AsNoTracking().Where(user => user.UID == userToken.Uid).ToListAsync();

            List<string> graphNames = new List<string>();

            foreach(User graph in graphsTiedToUser)
            {
                graphNames.Add(graph.Name ?? graph.GraphID);
            }

            return graphNames;
        }

        [Route("/api/removeGraph/{id}")]
        public async Task RemoveGraph(string id)
        {
            //Need to check if the graph is named first. If it is then we must get its ID.
            string graphID = await _affinityDbContext.Users.Where(graph => graph.Name == id).Select(graph => graph.GraphID).FirstOrDefaultAsync(); 

            if(graphID == null)
            {
                graphID = id;
            }

            var vertices = await _affinityDbContext.Vertices.Where(vertex => vertex.GraphID == graphID).ToListAsync();
            var edges = await _affinityDbContext.Edges.Where(vertex => vertex.GraphID == graphID).ToListAsync();

            _affinityDbContext.Vertices.RemoveRange(vertices);
            _affinityDbContext.Edges.RemoveRange(edges);
            _affinityDbContext.Users.Remove(_affinityDbContext.Users.Where(graph => graph.GraphID == graphID).FirstOrDefault());

            await _affinityDbContext.SaveChangesAsync();
        }

        private async Task<bool> GraphExistForUser(string uid, string graphID)
        {
            var userGraph = await _affinityDbContext.Users.Where(user => user.UID == uid && user.GraphID == graphID).FirstOrDefaultAsync();

            return userGraph != null;
        }

        private async Task CheckIfLoggedInAndMakeGraph(string graphID)
        {
            //If user is logged in
            if (await Utils.CheckFirebaseToken(_httpContextAccessor))
            {
                //Get the token!
                var userToken = await Utils.GetUserFirebaseToken(_httpContextAccessor);
                int graphsMadeByUser = await _affinityDbContext.Users.AsNoTracking().Where(user => user.UID == userToken.Uid).CountAsync();

                //User has less than the max amount of graphs saved
                if (graphsMadeByUser < GraphConstants.MAX_GRAPHS)
                {
                    await _affinityDbContext.AddAsync(new User { UID = userToken.Uid, GraphID = graphID, Modified = DateTime.Now });
                    await _affinityDbContext.SaveChangesAsync();
                }
            }
        }
    }
}
