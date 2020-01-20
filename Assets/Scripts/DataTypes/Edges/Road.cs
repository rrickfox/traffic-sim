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

        public Road(int id, RoadShape shape, Point point1, Point point2,
            IEnumerable<Lane> lanes2To1, IEnumerable<Lane> lanes1To2)
        {
            this.id = id;
            this.shape = shape;
            length = Vector2.Distance(point1.position, point2.position) / CONSTANTS.DISTANCE_UNIT;
            anchors.Add(AnchorNumber.One, new Anchor(point1, this, AnchorNumber.One, lanes2To1));
            anchors.Add(AnchorNumber.Two, new Anchor(point2, this, AnchorNumber.Two, lanes1To2));
        }
    }
}