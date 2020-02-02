using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataTypes
{
    public interface IVertex
    {
        ImmutableArray<Edge> edges { get; }
    }
    
    public class Vertex<TThis, TBehaviour> : GameObjectData<TThis, TBehaviour>, IVertex
        where TBehaviour : VertexBehaviour<TThis>
        where TThis : Vertex<TThis, TBehaviour>
    {
        public ImmutableArray<Edge> edges { get; }

        protected Vertex(IEnumerable<Edge> edges)
        {
            this.edges = edges.ToImmutableArray();
            foreach (var edge in edges)
            {
                edge.vertex = this;
            }
            
            CreateGameObject();
        }

        protected Vertex(params Edge[] edges) : this(edges.ToImmutableArray()) { }
    }

    public class VertexBehaviour<TData> : LinkedBehaviour<TData> { }
}