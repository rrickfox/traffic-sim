using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace DataTypes
{
    public interface IVertex
    {
        ImmutableArray<Edge> edges { get; }
    }
    
    public class BaseVertex<TThis, TBehaviour> : GameObjectData<TThis, TBehaviour>, IVertex
        where TBehaviour : VertexBehaviour<TThis>
        where TThis : BaseVertex<TThis, TBehaviour>
    {
        public ImmutableArray<Edge> edges { get; private set; }

        protected BaseVertex(IEnumerable<Edge> edges) => SetEdges(edges);
        protected BaseVertex(GameObject prefab, IEnumerable<Edge> edges) : base(prefab) => SetEdges(edges);
        
        private void SetEdges(IEnumerable<Edge> edges)
        {
            this.edges = edges.ToImmutableArray();
            foreach (var edge in this.edges)
            {
                edge.vertex = this;
            }
        }
    }
    
    public class InvisibleVertex<TThis, TBehaviour> : BaseVertex<TThis, TBehaviour>
        where TBehaviour : VertexBehaviour<TThis>
        where TThis : BaseVertex<TThis, TBehaviour>
    {
        protected InvisibleVertex(IEnumerable<Edge> edges) : base(edges) { }
        protected InvisibleVertex(params Edge[] edges) : this(edges.ToImmutableArray()) { }
    }

    public class VisualVertex<TThis, TBehaviour> : BaseVertex<TThis, TBehaviour>
        where TBehaviour : VertexBehaviour<TThis>
        where TThis : VisualVertex<TThis, TBehaviour>
    {
        protected GameObject _prefab { get; }

        protected VisualVertex(GameObject prefab, IEnumerable<Edge> edges) : base(prefab, edges) => _prefab = prefab;
        protected VisualVertex(GameObject prefab, params Edge[] edges) : this(prefab, edges.ToImmutableArray()) { }
    }

    public class VertexBehaviour<TData> : LinkedBehaviour<TData> where TData : IVertex { }
}