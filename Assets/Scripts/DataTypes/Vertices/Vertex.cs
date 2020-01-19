using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataTypes
{
    public class Vertex
    {
        private ImmutableArray<Anchor> anchors;

        public Vertex(IEnumerable<Anchor> anchors)
        {
            this.anchors = anchors.ToImmutableArray();
        }
    }
}