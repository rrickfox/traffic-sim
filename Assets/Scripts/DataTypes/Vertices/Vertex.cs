using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
        
        public static void StartPathfinding(List<Vertex> vertices)
        {
            foreach (var start in vertices.OfType<EndPoint>())
            {
                foreach (var end in vertices.OfType<EndPoint>().Where(end => end != start))
                {
                    start.FindPath(vertices, end);
                }
            }
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