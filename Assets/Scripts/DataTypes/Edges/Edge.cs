using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    // represents what you can tell about a road if you were to stand at one of its endpoints
    public class Edge
    {
        // the Vertex from which this edge originates
        public Vertex vertex = null;
        // represents how the road would look like from its other endpoint
        public Edge other { get; }
        // the cars on the outgoing side of the road
        public List<Car> cars { get; } = new List<Car>();
        public RoadShape shape { get; }
        // the coordinates of the end of the road from which you look at the road
        public RoadPoint originPoint => shape.points[0];
        public List<Lane> outgoingLanes { get; }
        // the incomingLanes of this are just the outgoingLanes of the other view
        public List<Lane> incomingLanes => other.outgoingLanes;
        public float length => shape.length;
        public float angle { get; }

        public Edge(RoadShape shape, Vector2 otherPosition,
            List<Lane> outgoingLanes, List<Lane> incomingLanes)
        {
            this.shape = shape;
            this.outgoingLanes = outgoingLanes;
            angle = Vector2.SignedAngle(otherPosition - originPoint, Vector2.right);
            other = new Edge(this, incomingLanes);
        }

        // construct an Edge where other is already constructed
        private Edge(Edge other, List<Lane> outgoingLanes)
        {
            this.other = other;
            this.outgoingLanes = outgoingLanes;
            this.shape = other.shape.Inverse();
            angle = this.other.angle;
        }
        
        public Vector2 GetAbsolutePosition(float positionOnRoad, float lane)
        {
            var index = Mathf.FloorToInt(positionOnRoad);
            var absolutePosition = shape.points[index];

            absolutePosition.position += Vector2.Lerp(absolutePosition.position, shape.points[index + 1].position, positionOnRoad - index);
            absolutePosition.forward += Vector2.Lerp(absolutePosition.forward, shape.points[index + 1].forward, positionOnRoad - index);

            // set offset to the right to accomodate different lanes
            var perpandicularOffset = (((this.outgoingLanes.Count + other.outgoingLanes.Count) / 2) - this.outgoingLanes.Count + 0.5f + lane) * CONSTANTS.LANE_WIDTH;

            // calculate backwards vector to rotate to right facing vector using Vector2.Perpendicular()
            var inverse = absolutePosition.forward * -1;

            absolutePosition.position += Vector2.Perpendicular(inverse) * perpandicularOffset;

            return absolutePosition.position;
        }
    }
}
