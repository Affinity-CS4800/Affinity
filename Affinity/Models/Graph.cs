using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using Newtonsoft.Json;

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
        public List<Edge> Edges { get; set; }
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
        public List<Vertex> AdjacencyList { get; set; }

        /// <summary>
        /// Adds a vertex to the adjacenecy list
        /// </summary>
        /// <param name="vertex"></param>
        public void AddVertex(Vertex vertex)
        {
            if(!VertexExists(vertex))
            {
                AdjacencyList.Add(vertex);
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
        public void AddEdge(Vertex fromVertex, Vertex toVertex, string name = "",
                            int color = GraphConstants.DEFAULT_EDGE_COLOR,
                            int weight = -1, Direction direction = Direction.Undirected)
        {
            if(VertexExists(fromVertex) && VertexExists(toVertex))
            {
                int vertexIndex = FindVertex(fromVertex);

                //Vertex not found
                //Add some sort of error thing???
                if(vertexIndex == -1)
                {
                    return;
                }

                AdjacencyList[vertexIndex].Edges.Add(new Edge { Color = color, Direction = direction, First = fromVertex.ID, Second = toVertex.ID, Weight = weight });
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
            int vertexIndex = FindVertex(fromVertex);

            if(vertexIndex == -1)
            {
                return -1;
            }

            for (int j = 0; j < AdjacencyList[vertexIndex].Edges.Count; j++)
            {
                if(AdjacencyList[vertexIndex].Edges[j].Direction == Direction.Undirected)
                {
                    return -1;
                }
                else
                {
                    if(AdjacencyList[vertexIndex].Edges[j].Second == toVertex.ID)
                    {
                        return AdjacencyList[vertexIndex].Edges[j].Weight;
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
                if(vertex.Name.Equals(vertexToCheck.Name))
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

        private int FindVertex(Vertex vertex)
        {
            for (int i = 0; i < VertexCount; i++)
            {
                if (AdjacencyList[i].ID == vertex.ID)
                {
                    return i;
                }
            }
            return -1;
        }

        public string PrintAdjacencyList()
        {
            return JsonConvert.SerializeObject(AdjacencyList);
        }
    }
}
