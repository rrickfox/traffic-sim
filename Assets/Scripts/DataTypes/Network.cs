using System.Collections.Generic;

namespace DataTypes
{
    public class Network
    {
        public List<Vertex> vertices { get; }
        public List<Edge> edges { get; }

        public Network(List<Vertex> vertices, List<Edge> edges)
        {
            this.vertices = vertices;
            this.edges = edges;
        }
    }
}