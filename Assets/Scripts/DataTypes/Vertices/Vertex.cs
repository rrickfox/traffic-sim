using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataTypes
{
    public class Vertex
    {
        private ImmutableArray<Edge> _anchors;

        public Vertex(IEnumerable<Edge> anchors)
        {
            _anchors = anchors.ToImmutableArray();
        }
    }
}