using UnityEngine;
using System;
namespace DataTypes
{
    public class Road
    {
        public readonly int id;
        public Anchor anchors1;
        public Anchor anchors2;
        public int lanes1To2;
        public int lanes2To1;
        public float length;
        public RoadShape shape;

        private CONSTANTS _constants = new CONSTANTS();

        public Road(int id, Vector2 position1, Vector2 position2, int lanes1To2, int lanes2To1)
        {
            this.id = id;
            anchors1 = new Anchor(position1, lanes1To2, lanes2To1);
            anchors2 = new Anchor(position2, lanes2To1, lanes1To2);
            this.lanes1To2 = lanes1To2;
            this.lanes2To1 = lanes2To1;
            length = Vector2.Distance(position1, position2) / _constants.DISTANCE_UNIT;
        }
    }
}