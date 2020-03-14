using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;
using Events;

namespace DataTypes
{
    public abstract class Vertex : GameObjectData
    {
        public ImmutableArray<Edge> edges { get; private set; }
        public Dictionary<RouteSegment, Dictionary<int, SectionTrack>> routes { get; set; }
        // updates only happen after all car updates
        public static TypePublisher typePublisher { get; } = new TypePublisher(Car.typePublisher);
        
        protected Vertex(IEnumerable<Edge> edges) => SetEdges(edges);
        // constructor aliases that use a variable amount of parameters instead of an enumerable
        protected Vertex(params Edge[] edges) : this(edges.ToImmutableArray()) { }
        
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
}