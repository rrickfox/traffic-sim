using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace DataTypes
{
    public interface IVertex : IGameObjectData
    {
        ImmutableArray<Edge> edges { get; }
        LaneType SubRoute(Edge comingFrom, Edge to);
    }
    
    public abstract class Vertex<TThis, TBehaviour> : GameObjectData<TThis, TBehaviour>, IVertex
        where TBehaviour : VertexBehaviour<TThis>
        where TThis : Vertex<TThis, TBehaviour>
    {
        public ImmutableArray<Edge> edges { get; private set; }
        
        protected Vertex(IEnumerable<Edge> edges) => SetEdges(edges);
        protected Vertex(GameObject prefab, IEnumerable<Edge> edges) : base(prefab) => SetEdges(edges);
        // constructor aliases using a variable amount of parameters instead of an enumerable
        protected Vertex(params Edge[] edges) : this(edges.ToImmutableArray()) { }
        protected Vertex(GameObject prefab, params Edge[] edges) : this(prefab, edges.ToImmutableArray()) { }
        
        private void SetEdges(IEnumerable<Edge> edges)
        {
            this.edges = edges.ToImmutableArray();
            foreach (var edge in this.edges)
            {
                edge.vertex = this;
            }
        }

        public abstract LaneType SubRoute(Edge comingFrom, Edge to);
    }

    public class VertexBehaviour<TData> : LinkedBehaviour<TData> where TData : IVertex { }
}