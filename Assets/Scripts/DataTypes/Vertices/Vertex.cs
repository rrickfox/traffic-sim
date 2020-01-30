using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataTypes
{
    public class Vertex
    {
        private ImmutableArray<Edge> _edges;

        protected Vertex(IEnumerable<Edge> edges)
        {
            _edges = edges.ToImmutableArray();
        }
    }
}