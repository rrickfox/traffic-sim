using System.Collections.Generic;

namespace DataTypes
{
    public class Roundabout : Vertex
    {
        private Roundabout(IEnumerable<Edge> anchors) : base(anchors)
        {
        }
    }
}