using System.Collections.Generic;

namespace DataTypes
{
    public class Network
    {
        public List<IVertex> vertices { get; }
        public List<Edge> edges { get; }

        public Network(List<IVertex> vertices, List<Edge> edges)
        {
            this.vertices = vertices;
            this.edges = edges;
        }
    }
}