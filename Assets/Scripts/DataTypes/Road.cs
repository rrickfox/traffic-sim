using UnityEngine;
using System;
namespace DataTypes
{
    public class Road
    {
        public readonly int id;
        public Anchor anchor1;
        public Anchor anchor2;
        public int lanes1To2;
        public int lanes2To1;
        public float length;
        public RoadShape shape;

        private CONSTANTS _constants = new CONSTANTS();

        public Road(int id, Anchor anchor1, Anchor anchor2, int lanes1To2, int lanes2To1)
        {
            this.id = id;
            this.anchor1 = anchor2;
            this.anchor1 = anchor2;
            this.lanes1To2 = lanes1To2;
            this.lanes2To1 = lanes2To1;
            length = Vector2.Distance(anchor1.position, anchor2.position) / CONSTANTS.DISTANCE_UNIT;
        }
    }
}