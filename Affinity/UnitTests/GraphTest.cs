/*using Affinity.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Affinity.UnitTests
{
    public class GraphTest
    {
        private readonly Graph graph;

        public GraphTest()
        {
            graph = new Graph();

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
                graph.AddEdge(edge.First, edge.Second, "", edge.Color, edge.Weight, edge.Direction);
            }
        }

        [Fact]
        public void TestGetNeighbors()
        {
            
        }

        [Fact]
        public void TestDijkstra()
        {

        }

        [Fact]
        public void TestBFS()
        {

        }

        [Fact]
        public void TestDistance()
        {
            Assert.Equal(2, graph.CalculateGraphDistance(graph.AdjacencyList[0], graph.AdjacencyList[1]));
            Assert.Equal(1, graph.CalculateGraphDistance(graph.AdjacencyList[0], graph.AdjacencyList[2]));
            Assert.Equal(1, graph.CalculateGraphDistance(graph.AdjacencyList[0], graph.AdjacencyList[3]));
        }

        [Fact]
        public void TestDiameter()
        {
            Assert.Equal(2, graph.CalculateGraphDiameter(graph));
        }
    }
}
*/