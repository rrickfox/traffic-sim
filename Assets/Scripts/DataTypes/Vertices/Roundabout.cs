using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    public class Roundabout : Vertex
    {
        private Roundabout(GameObject prefab, IEnumerable<Edge> edges) : base(prefab, edges)
        {
        }

        public override LaneType SubRoute(Edge comingFrom, Edge to)
        {
            throw new System.NotImplementedException();
        }
    }
}