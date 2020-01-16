using UnityEngine;
using System;
namespace DataTypes
{
    public class Road
    {
        public readonly int id;
        public Node node1;
        public Node node2;
        public int lanes1To2;
        public int lanes2To1;
        public float length;
        public RoadShape shape;

        private CONSTANTS _constants = new CONSTANTS();

        public Road(int id, Node node1, Node node2, int lanes1To2, int lanes2To1)
        {
            this.id = id;
            this.node1 = node1;
            this.node2 = node2;
            this.lanes1To2 = lanes1To2;
            this.lanes2To1 = lanes2To1;
            length = Vector2.Distance(node1.position, node2.position) / _constants.DISTANCE_UNIT;
        }
    }
}