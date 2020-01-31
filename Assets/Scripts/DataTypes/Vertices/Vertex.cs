using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataTypes
{
    public class Vertex
    {
        private ImmutableArray<Edge> _edges;
        // distance value relative to start point of pathfinding
        private float? _pathDistance;
        
        public float? pathDistance
        {
            get => _pathDistance;
            
            set => _pathDistance = value;
        }
        
        // current candidate for predecessor in path
        private Vertex _previousVertex;
        
        public Vertex previousVertex
        {
            get => _previousVertex;
            
            set => _previousVertex = value;
        }

        // checks neighbourhood for necessary updates in pathfinding attributes
        public void CheckNeigbourhood()
        {
            foreach (var edge in _edges)
            {
                var tempDistance = pathDistance + edge.length;
                if (edge.other.vertex.pathDistance > tempDistance | edge.other.vertex.pathDistance == null)
                {
                    edge.other.vertex.pathDistance = tempDistance;
                    edge.other.vertex.previousVertex = this;
                }
            }
        }
        protected Vertex(IEnumerable<Edge> edges)
        {
            _edges = edges.ToImmutableArray();
            foreach (var edge in _edges)
            {
                edge.vertex = this;
            }
        }

        protected Vertex(params Edge[] edges) : this(edges.ToImmutableArray()) { }
    }
}