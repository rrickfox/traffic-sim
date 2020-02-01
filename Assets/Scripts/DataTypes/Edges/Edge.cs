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
        public Vector2 position { get; }
        public List<Lane> outgoingLanes { get; }
        // the incomingLanes of this are just the outgoingLanes of the other view
        public List<Lane> incomingLanes => other.outgoingLanes;
        public float length => Vector2.Distance(position, other.position) / CONSTANTS.DISTANCE_UNIT;
        public float angle => Vector2.SignedAngle(other.position - position, Vector2.right);

        public Edge(RoadShape shape, Vector2 position, Vector2 otherPosition,
            List<Lane> outgoingLanes, List<Lane> incomingLanes)
        {
            other = new Edge(this, shape, otherPosition, incomingLanes);
            this.shape = shape;
            this.position = position;
            this.outgoingLanes = outgoingLanes;
        }

        // construct an Edge without creating other
        private Edge(Edge other, RoadShape shape, Vector2 position, List<Lane> outgoingLanes)
        {
            this.other = other;
            this.shape = shape;
            this.position = position;
            this.outgoingLanes = outgoingLanes;
        }
        
        public Vector2 GetAbsolutePosition(float positionOnRoad, float lane)
        {
            var absolutePosition = Vector2.Lerp(this.position, other.position, positionOnRoad / length);
            // set offset to the right to accomodate different lanes
            var perpandicularOffset = (((this.outgoingLanes.Count + other.outgoingLanes.Count) / 2) - this.outgoingLanes.Count + 0.5f + lane) * CONSTANTS.LANE_WIDTH;

            // calculate backwards vector to rotate to right facing vector using Vector2.Perpendicular()
            var inverse = (other.position - this.position).normalized * -1;

            absolutePosition += Vector2.Perpendicular(inverse) * perpandicularOffset;

            return absolutePosition;
        }
    }
}
