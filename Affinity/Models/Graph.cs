using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Newtonsoft.Json;
using FibonacciHeap;
using System.Diagnostics;

public enum Direction
{
    Undirected,
    DirectedAtFirst,
    DirectedAtSecond
}

namespace Affinity.Models
{
    public class Vertex
    {
        [Key]
        public int DBID { get; set; }

        public string GraphID { get; set; }

        public int ID { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int Color { get; set; }
        [StringLength(8)] 
        public string Name { get; set; }
        [StringLength(7)]
        public string FontColor { get; set; }

        [NotMapped]
        public ICollection<Edge> Edges { get; set; }
    }

    public class Edge
    {
        [Key]
        public int DBID { get; set; }

        public string GraphID { get; set; }

        public int First { get; set; }
        public int Second { get; set; }
        public int Direction { get; set; }
        public int Color { get; set; }
        public string FontAlignment { get; set; }

        [Range(-1,2000)]
        public int Weight { get; set; }
        public string Name { get; set; }
    }

    public static class GraphConstants
    {
        public static int GRID_MAX = 3000;
        public static int VERTEX_MAX = 3201;
        public static int EDGE_MAX = 3200;

        public const int DEFAULT_EDGE_COLOR = -16777216; //ARGB For BLACK

        public const int MAX_GRAPHS = 5;
    }

    public class GraphIDName
    {
        public string GraphID { get; set; }
        public string Name { get; set; }
    }

    public class GraphDataJson
    {
        public List<Vertex> Vertices { get; set; }
        public List<Edge> Edges { get; set; }
    }

    //Directed graph, Undirected graph, multigraph?
    //Make it so its assumed to be a directed graph but does not need to be?
    public class Graph
    {
        public int VertexCount => AdjacencyList.Count;
        private int EdgeCount;
        private bool isDirected;
        public List<Vertex> AdjacencyList { get; set; }

        public Graph()
        {
            AdjacencyList = new List<Vertex>();
        }
        /*
        /// <summary>
        /// Adds a vertex to the adjacenecy list
        /// </summary>
        /// <param name="vertex"></param>
        public void AddVertex(Vertex vertex)
        {
            if (!VertexExists(vertex) && VertexCount < GraphConstants.VERTEX_MAX)
            {
                AdjacencyList.Add(vertex);
                AdjacencyList[VertexCount - 1].Edges = new List<Edge>();
            }
        }

        /// <summary>
        /// This function adds an edge to the adjacency list by taking in
        /// 2 vertex's one being the one we want to connect from and the other
        /// being the one to connect to. The fuction doesnt need to know anything
        /// about the edge, but it can be specified. It will default to a black
        /// edge with no name and not directed so a weight of -1.
        /// </summary>
        /// <param name="fromVertexID"></param>
        /// <param name="toVertexID"></param>
        /// <param name="name"></param>
        /// <param name="color"></param>
        /// <param name="weight"></param>
        /// <param name="direction"></param>
        public void AddEdge(int fromVertexID, int toVertexID, string name = "",
                            int color = GraphConstants.DEFAULT_EDGE_COLOR,
                            int weight = -1, Direction direction = Direction.Undirected)
        {
            if (VertexExists(AdjacencyList[FindVertex(fromVertexID)]) && VertexExists(AdjacencyList[FindVertex(toVertexID)]) && EdgeCount < GraphConstants.EDGE_MAX)
            {
                //Vertex exists no need to check if its -1 or not
                int vertexIndex = FindVertex(fromVertexID);

                isDirected |= direction == Direction.DirectedAtFirst || direction == Direction.DirectedAtSecond;

                AdjacencyList[vertexIndex].Edges.Add(new Edge { Name = name, Color = color, Direction = direction, First = fromVertexID, Second = toVertexID, Weight = weight });
            }
        }

        /// <summary>
        /// Returns the weight between the two specified vertices
        /// </summary>
        /// <param name="fromVertex"></param>
        /// <param name="toVertex"></param>
        /// <returns></returns>
        public int GetWeight(Vertex fromVertex, Vertex toVertex)
        {
            int vertexIndex = FindVertex(fromVertex.ID);

            if (vertexIndex == -1)
            {
                return -1;
            }

            //Needed to access index of a ICollection
            var edgeArray = AdjacencyList[vertexIndex].Edges.ToArray();

            for (int j = 0; j < AdjacencyList[vertexIndex].Edges.Count; j++)
            {
                if (edgeArray[j].Direction == Direction.Undirected)
                {
                    return -1;
                }
                else
                {
                    if (edgeArray[j].Second == toVertex.ID)
                    {
                        return edgeArray[j].Weight;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Checks if the vertex doesn't already exist in the adjacency list
        /// </summary>
        /// <param name="vertexToCheck"></param>
        /// <returns></returns>
        private bool VertexExists(Vertex vertexToCheck)
        {
            foreach (var vertex in AdjacencyList)
            {
                if (vertex.ID.Equals(vertexToCheck.ID))
                {
                    return true;
                }
            }
            return false;
        }

        public void SaveGraph()
        {

        }

        public void InitGraphFromDB()
        {

        }

        private int FindVertex(int vertexID)
        {
            for (int i = 0; i < VertexCount; i++)
            {
                if (AdjacencyList[i].ID == vertexID)
                {
                    return i;
                }
            }
            return -1;
        }

        public string PrintAdjacencyList()
        {
            return JsonConvert.SerializeObject(AdjacencyList, Formatting.Indented);
        }

        //Used for finding the Graph geodesic on a weighted graph
        //Using a Fibonacci Heap gives us O(E + V log V)
        public Tuple<int[], int[]> Dijkstra(Vertex startVertex)
        {
            var watch = Stopwatch.StartNew();

            int[] dist = new int[VertexCount]; //distance
            int[] prev = new int[VertexCount]; //predecessors of v

            dist[startVertex.ID] = 0;

            //Our priority queue made of a fibonacci heap for better performance
            FibonacciHeap<Vertex, int> priorityQueue = new FibonacciHeap<Vertex, int>(int.MinValue);

            //Keep track of the fibonacci nodes
            List<FibonacciHeapNode<Vertex, int>> fibonacciHeapNodes = new List<FibonacciHeapNode<Vertex, int>>();

            //Init the distance and the prev arrays
            foreach (Vertex v in AdjacencyList)
            {
                if (v.ID != startVertex.ID)
                {
                    dist[v.ID] = int.MaxValue; //Infinity
                }
                prev[v.ID] = -1; //Undefined predecessor

                FibonacciHeapNode<Vertex, int> heapNode = new FibonacciHeapNode<Vertex, int>(v, dist[v.ID]);

                priorityQueue.Insert(heapNode);
                fibonacciHeapNodes.Add(heapNode);
            }

            while (!priorityQueue.IsEmpty())
            {
                var u = priorityQueue.RemoveMin().Data; //Gets the vertex that has the min value in the p-queue

                foreach (Vertex v in GetNeighbors(u))
                {
                    var alt = dist[u.ID] + Length(u, v);
                    if (alt < dist[v.ID])
                    {
                        dist[v.ID] = alt;
                        prev[v.ID] = u.ID;
                        priorityQueue.DecreaseKey(fibonacciHeapNodes[v.ID], alt);
                    }
                }
            }

            watch.Stop();

            Debug.WriteLine($"Dijkstra took: {watch.ElapsedMilliseconds}ms");

            return Tuple.Create(dist, prev);
        }

        public List<Vertex> GetNeighbors(Vertex vertex)
        {
            List<Vertex> neighboringVertices = new List<Vertex>();

            //For each edge that is attached to this vertex
            foreach (Edge edge in AdjacencyList[FindVertex(vertex.ID)].Edges)
            {
                //Directed at us so it is not a neighboring vertex -> Move to next
                if (edge.Direction == Direction.DirectedAtFirst)
                {
                    continue;
                }
                //If the edge has no direction or we are currently pointing to the other vertex we should add!
                else if (edge.Direction == Direction.DirectedAtSecond || edge.Direction == Direction.Undirected)
                {
                    neighboringVertices.Add(AdjacencyList[FindVertex(edge.Second)]);
                }
            }

            return neighboringVertices;
        }

        private int Length(Vertex v, Vertex u)
        {
            foreach (Edge edge in v.Edges)
            {
                if (edge.First == v.ID && edge.Second == u.ID)
                {
                    return edge.Weight;
                }
            }

            return 0;
        }

        //Used for finding the Graph geodesic on an unweighted graph
        //TODO::FINISH IMPLEMENTING
        public int[] BFS(Vertex vertex)
        {
            bool[] visited = new bool[VertexCount];
            Queue<Vertex> vertexQueue = new Queue<Vertex>();

            vertexQueue.Enqueue(vertex);
            visited[vertex.ID] = true;

            while (vertexQueue.Count != 0)
            {
                Vertex v = vertexQueue.Dequeue();

                if (!visited[v.ID])
                {
                    visited[v.ID] = true;

                    foreach (Edge edge in v.Edges)
                    {
                        if (!visited[edge.Second])
                        {
                            vertexQueue.Enqueue(AdjacencyList[edge.Second]);
                        }
                    }
                }
            }

            return null;
        }

        public int CalculateGraphDistance(Vertex u, Vertex v)
        {
            int distance = 0;

            if (isDirected)
            {
                //Perform dijkstra to find the pathing for smallest path
                var dijkstra = Dijkstra(u);

                int parent = dijkstra.Item2[v.ID];

                //Cant get there from this node!
                if (parent == -1)
                {
                    return 0;
                }

                if (parent == u.ID)
                {
                    return 1;
                }

                while (parent != u.ID)
                {
                    parent = dijkstra.Item2[parent];
                    distance++;
                }

                return distance;
            }
            else
            {
                var parents = BFS(u);

                return distance;
            }

        }

        public int CalculateGraphDiameter(Graph graph)
        {
            int diameter = 0;

            foreach (Vertex vertex in AdjacencyList)
            {
                Debug.WriteLine($"Checking Dijkstra on {vertex.Name}");
                var dijkstra = graph.Dijkstra(vertex);

                foreach (Vertex otherVertex in AdjacencyList)
                {
                    if (vertex.ID == otherVertex.ID)
                    {
                        continue;
                    }

                    int distance = graph.CalculateGraphDistance(vertex, otherVertex);

                    Debug.WriteLine(distance);

                    if (distance > diameter)
                    {
                        diameter = distance;
                    }
                }
            }
            return diameter;
        }*/
    }
}
