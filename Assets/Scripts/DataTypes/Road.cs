using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    public class Road
    {
        public readonly int id;
        public Dictionary<AnchorNumber, Anchor> anchors;
        public float length;
        public RoadShape shape;

        public Road(int id, Anchor anchor1, Anchor anchor2, RoadShape shape)
        {
            this.id = id;
            anchor1.parentRoad = this;
            anchor2.parentRoad = this;
            anchors.Add(AnchorNumber.One, anchor1);
            anchors.Add(AnchorNumber.Two, anchor2);
            length = Vector2.Distance(anchor1.position, anchor2.position) / CONSTANTS.DISTANCE_UNIT;
            this.shape = shape;
        }
    }
}