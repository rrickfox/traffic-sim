using System.Collections.Generic;

namespace DataTypes
{
    public class Roundabout : Vertex<Roundabout, RoundaboutBehaviour>
    {
        private Roundabout(IEnumerable<Edge> edges) : base(edges)
        {
            CreateGameObject();
        }
    }

    public class RoundaboutBehaviour : VertexBehaviour<Roundabout> { }
}