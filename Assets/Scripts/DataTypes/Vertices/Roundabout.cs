using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    public class Roundabout : Vertex<Roundabout, RoundaboutBehaviour>
    {
        private Roundabout(GameObject prefab, IEnumerable<Edge> edges) : base(prefab, edges)
        {
        }

        public override LaneType SubRoute(Edge comingFrom, Edge to)
        {
            throw new System.NotImplementedException();
        }
    }

    public class RoundaboutBehaviour : VertexBehaviour<Roundabout> { }
}