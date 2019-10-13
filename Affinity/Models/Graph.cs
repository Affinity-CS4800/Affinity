using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using Newtonsoft.Json;
using FibonacciHeap;

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
        public int ID { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int Color { get; set; }
        [StringLength(8)]
        public string Name { get; set; }

        public ICollection<Edge> Edges { get; set; }
    }

    public class Edge
    {
        public int ID { get; set; }
        public int First { get; set; }
        public int Second { get; set; }
        [EnumDataType(typeof(Direction))]
        public Direction Direction { get; set; }
        public int Color { get; set; }
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
    }

    //Directed graph, Undirected graph, multigraph?
    //Make it so its assumed to be a directed graph but does not need to be?
    public class Graph
    {
        public int VertexCount => AdjacencyList.Count;
        private int EdgeCount;
        public List<Vertex> AdjacencyList { get; set; }

        public Graph()
        {
            AdjacencyList = new List<Vertex>();
        }

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
        /// <param name="fromVertex"></param>
        /// <param name="toVertex"></param>
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
                int vertexIndex = FindVertex(fromVertexID);

                //Vertex not found
                //Add some sort of error thing???
                if (vertexIndex == -1)
                {
                    return;
                }

                AdjacencyList[vertexIndex].Edges.Add(new Edge { Name = name, Color = color, Direction = direction, First = fromVertexID, Second = toVertexID, Weight = weight, ID = EdgeCount++ });
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


        //1     function Dijkstra(Graph, source):
        //2      dist[source] ← 0                           // Initialization
        //3
        //4      create vertex priority queue Q
        //5
        //6      for each vertex v in Graph:           
        //7          if v ≠ source
        //8              dist[v] ← INFINITY                 // Unknown distance from source to v
        //9          prev[v] ← UNDEFINED                    // Predecessor of v
        //10
        //11         Q.add_with_priority(v, dist[v])
        //12
        //13
        //14     while Q is not empty:                      // The main loop
        //15         u ← Q.extract_min()                    // Remove and return best vertex
        //16         for each neighbor v of u:              // only v that are still in Q
        //17             alt ← dist[u] + length(u, v)
        //18             if alt<dist[v]
        //19                 dist[v] ← alt
        //20                 prev[v] ← u
        //21                 Q.decrease_priority(v, alt)
        //22
        //23     return dist, prev

        //Used for finding the Graph geodesic on a weighted graph
        //Using a Fibonacci Heap gives us O(E + V log V)
        public Tuple<int[],int[]> Dijkstra(Vertex startVertex)
        {
            int[] dist = new int[VertexCount]; //distance
            int[] prev = new int[VertexCount]; //predecessors of v

            dist[startVertex.ID] = 0;

            FibonacciHeap<Vertex,int> priorityQueue = new FibonacciHeap<Vertex,int>(int.MinValue);

            //Init the distance and the prev arrays
            foreach(Vertex v in AdjacencyList)
            {
                if(v.ID != startVertex.ID)
                {
                    dist[v.ID] = int.MaxValue; //Infinity
                }
                prev[v.ID] = -1; //Undefined predecessor

                priorityQueue.Insert(new FibonacciHeapNode<Vertex,int>(v, dist[v.ID]));
            }

            while(!priorityQueue.IsEmpty())
            {
                var u = priorityQueue.RemoveMin().Data; //Gets the vertex that has the min value in the p-queue

                foreach (Vertex v in GetNeighbors(u))
                {
                    var alt = dist[u.ID]; // + length(u,v);
                    if (alt < dist[v.ID])
                    {
                        dist[v.ID] = alt;
                        prev[v.ID] = u.ID;
                        priorityQueue.DecreaseKey(new FibonacciHeapNode<Vertex, int>(v, dist[v.ID]), alt);
                    }
                }
            }

            return Tuple.Create(dist, prev);
        }

        public List<Vertex> GetNeighbors(Vertex vertex)
        {
            List<Vertex> neighboringVertices = new List<Vertex>();

            //For each edge that is attached to this vertex
            foreach(Edge edge in AdjacencyList[FindVertex(vertex.ID)].Edges)
            {
                //Directed at us so it is not a neighboring vertice!
                if(edge.Direction == Direction.DirectedAtFirst)
                {
                    continue;
                }
                //If the edge has no direction or we are currently pointing to the other vertex we should add!
                else if(edge.Direction == Direction.DirectedAtSecond || edge.Direction == Direction.Undirected)
                {
                    neighboringVertices.Add(AdjacencyList[FindVertex(edge.Second)]);
                }
            }

            return neighboringVertices;
        }

        //Used for finding the Graph geodesic on an unweighted graph
        public void BFS()
        {

        }

        public int CalculateGraphDistance()
        {
            return 0;
        }
    }
}
