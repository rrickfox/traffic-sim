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

        public Road(int id, Vector2 position1, Vector2 position2, int lanes1To2, int lanes2To1)
        {
            this.id = id;
            node1 = new Node(position1, lanes1To2, lanes2To1);
            node2 = new Node(position2, lanes2To1, lanes1To2);
            this.lanes1To2 = lanes1To2;
            this.lanes2To1 = lanes2To1;
            length = Vector2.Distance(position1, position2) / _constants.DISTANCE_UNIT;
        }
    }
}