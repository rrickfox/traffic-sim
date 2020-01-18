using System.Collections.Generic;

namespace DataTypes
{
    public class Roundabout : Vertex
    {
        public Roundabout(IEnumerable<Anchor> anchors) : base(anchors)
        {
        }
    }
}