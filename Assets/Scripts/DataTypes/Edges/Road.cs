using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    public class Road
    {
        public readonly int id;
        public Dictionary<AnchorNumber, Anchor> anchors = new Dictionary<AnchorNumber, Anchor>();
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

        // calculate the position on the road when given all the parameters
        public Vector2 GetPosition(float positionOnRoad, Direction direction, float lane)
        {
            Vector2 position = new Vector2();
            Vector2 backward = new Vector2();
            float offset = 0;
            if(direction == Direction.direction1To2)
            {
                position = Vector2.Lerp(anchors[AnchorNumber.One].position, anchors[AnchorNumber.Two].position, positionOnRoad);
                // set offset to the right to accomodate different lanes
                offset = (((anchors[AnchorNumber.Two].endingLanes.Length + anchors[AnchorNumber.One].endingLanes.Length) / 2) - anchors[AnchorNumber.Two].endingLanes.Length + 0.5f + lane) * CONSTANTS.LANE_WIDTH;

                // calculate backwards vector to rotate to right facing vector using Vector2.Perpendicular()
                backward = (anchors[AnchorNumber.Two].position - anchors[AnchorNumber.One].position).normalized * -1;

                position += Vector2.Perpendicular(backward) * offset;
            }
            else
            {
                position = Vector2.Lerp(anchors[AnchorNumber.Two].position, anchors[AnchorNumber.One].position, positionOnRoad);
                offset = (((anchors[AnchorNumber.Two].endingLanes.Length + anchors[AnchorNumber.One].endingLanes.Length) / 2) - anchors[AnchorNumber.One].endingLanes.Length + 0.5f + lane) * CONSTANTS.LANE_WIDTH;

                backward = (anchors[AnchorNumber.One].position - anchors[AnchorNumber.Two].position).normalized * -1;
                
                position += backward * offset;
            }

            return position;
        }
    }
}
