using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace DataTypes
{
    public class Roundabout : Vertex
    {
        public override GameObject prefab { get; } = CONSTANTS.EMPTY_PREFAB;

        private Roundabout(IEnumerable<Edge> edges) : base(edges)
        {
        }

        public override LaneType SubRoute(Edge comingFrom, Edge to)
        {
            throw new System.NotImplementedException();
        }
    }
}