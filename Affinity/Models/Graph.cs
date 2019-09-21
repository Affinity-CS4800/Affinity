using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

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
    }
}
