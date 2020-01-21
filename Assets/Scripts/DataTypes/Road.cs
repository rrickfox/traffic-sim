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
            length = Vector2.Distance(node1.position, node2.position) / CONSTANTS.DISTANCE_UNIT;
        }

        // returns position of car on Road, when given progress on Road, driving direction and lane
        public Vector2 GetPosition(float positionOnRoad, Direction direction, float lane)
        {
            Vector2 position = new Vector2();
            Vector2 backwards = new Vector2();
            float offset = 0;
            if(direction == Direction.direction1To2)
            {
                // get linear position, ignoring lane position and driving direction
                position = Vector2.Lerp(node1.position, node2.position, positionOnRoad);
                // set offset to accomodate different lanes and driving direction
                offset = (((lanes1To2 + lanes2To1) / 2) - lanes1To2 + 0.5f + lane) * CONSTANTS.LANE_WIDTH;
                // has to be backwards, because Vector2.Perpendicular returns vector turned counter-clockwise
                backwards = (node2.position - node1.position).normalized * -1;

                // add offset to the right to position
                position += Vector2.Perpendicular(backwards) * offset; 
            }
            else
            {
                position = Vector2.Lerp(node2.position, node1.position, positionOnRoad);
                offset = (((lanes1To2 + lanes2To1) / 2) - lanes2To1 + 0.5f + lane) * CONSTANTS.LANE_WIDTH;

                backwards = (node1.position - node2.position).normalized * -1;
                
                position += Vector2.Perpendicular(backwards) * offset;
            }

            return position;
        }
    }
}