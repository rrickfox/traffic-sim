using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MoreLinq;

namespace DataTypes
{
    public class Vertex
    {
        private ImmutableArray<Edge> _edges;
        // distance value relative to start point of pathfinding
        public float? pathDistance { get; protected set; }
        // current candidate for predecessor in path
        public Vertex previousVertex { get; private set; }

        protected Vertex(IEnumerable<Edge> edges)
        {
            _edges = edges.ToImmutableArray();
            foreach (var edge in _edges)
            {
                edge.vertex = this;
            }
        }

        protected Vertex(params Edge[] edges) : this(edges.ToImmutableArray()) { }

        public static void StartPathfinding(ICollection<Vertex> vertices)
        {
            var verticesSet = vertices.ToHashSet();
            var endPoints = vertices.OfType<EndPoint>().ToList();
            foreach (var start in endPoints)
            {
                foreach (var end in endPoints.Where(end => end != start))
                {
                    start.FindPath(verticesSet, end);
                    foreach (var vertex in verticesSet)
                    {
                        vertex.pathDistance = null;
                        vertex.previousVertex = null;
                    }
                }
            }
        }

        // checks neighbourhood for necessary updates in pathfinding attributes
        public void CheckNeigbourhood()
        {
            foreach (var edge in _edges.Where(edge => edge.outgoingLanes.Count > 0))
            {
                var tempDistance = pathDistance + edge.length;
                if (edge.other.vertex.pathDistance > tempDistance | edge.other.vertex.pathDistance == null)
                {
                    edge.other.vertex.pathDistance = tempDistance;
                    edge.other.vertex.previousVertex = this;
                }
            }
        }
                
        public Edge GetEdge(Vertex neigbour)
        {
            return _edges.FirstOrDefault(edge => edge.other.vertex == neigbour);
        }
    }
}