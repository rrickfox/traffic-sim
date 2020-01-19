using System.Collections.Generic;

namespace DataTypes
{
    public class Roundabout : Vertex
    {
        private Roundabout(IEnumerable<Anchor> anchors) : base(anchors)
        {
        }
    }
}