using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DataTypes
{
    public interface IVertex
    {
        float? pathDistance { get; set; }
        IVertex previousVertex { get; set; }
        void CheckNeigbourhood();
        Edge GetEdge(IVertex neighbour);
    }
    
    public class Vertex<TThis, TBehaviour> : GameObjectData<TThis, TBehaviour>, IVertex
        where TBehaviour : VertexBehaviour<TThis>
        where TThis : Vertex<TThis, TBehaviour>
    {
        private ImmutableArray<Edge> _edges { get; }
        // distance value relative to start point of pathfinding
        public float? pathDistance { get; set; }
        // current candidate for predecessor in path
        public IVertex previousVertex { get; set; }

        protected Vertex(IEnumerable<Edge> edges)
        {
            _edges = edges.ToImmutableArray();
            foreach (var edge in _edges)
            {
                edge.vertex = this;
            }
        }

        protected Vertex(params Edge[] edges) : this(edges.ToImmutableArray()) { }

        // checks neighbourhood for necessary updates in pathfinding attributes
        public void CheckNeigbourhood()
        {
            foreach (var edge in _edges.Where(edge => edge.outgoingLanes.Count > 0 
                                                      && (edge.other.vertex.pathDistance == null || 
                                                          edge.other.vertex.pathDistance > pathDistance + edge.length)))
            {
                edge.other.vertex.pathDistance = pathDistance + edge.length;
                edge.other.vertex.previousVertex = this;
            }
        }

        public Edge GetEdge(IVertex neighbour)
        {
            return _edges.FirstOrDefault(edge => edge.other.vertex == neighbour);
        }
    }

    public class VertexBehaviour<TData> : LinkedBehaviour<TData> where TData : IVertex { }
}