using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataTypes
{
    public class Vertex
    {
        private ImmutableArray<Anchor> _anchors;

        public Vertex(IEnumerable<Anchor> anchors)
        {
            _anchors = anchors.ToImmutableArray();
        }
    }
}