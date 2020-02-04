using System.Collections.Generic;
using UnityEngine;
using Utility;

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

        public Edge(RoadShape shape, List<Lane> outgoingLanes, List<Lane> incomingLanes)
        {
            this.shape = shape;
            this.outgoingLanes = outgoingLanes;
            other = new Edge(this, incomingLanes);
        }

        // construct an Edge where other is already constructed
        private Edge(Edge other, List<Lane> outgoingLanes)
        {
            this.other = other;
            this.outgoingLanes = outgoingLanes;
            this.shape = other.shape.Inverse();
        }
        
        // retrieves position and forward vector of car on road when given relative position on road and lane
        public RoadPoint GetAbsolutePosition(float positionOnRoad, float lane)
        {
            // get first estimation of position from saved array of points
            positionOnRoad = Mathf.Clamp(positionOnRoad, 0, length);
            var index = Mathf.FloorToInt(positionOnRoad);
            var absolutePosition = shape.points[index];

            // if positionOnRoad is behind last evenly spaced Point, then other rules apply
            if(index == shape.points.Length - 1)
            {
                // linearly interpolate between point and lastPoint
                absolutePosition.position += (shape.endingPoint.position - absolutePosition.position).normalized * (positionOnRoad - index) * CONSTANTS.DISTANCE_UNIT;
                absolutePosition.forward = (absolutePosition.forward * (positionOnRoad - index) + shape.endingPoint.forward * (1 - (positionOnRoad - index))).normalized;
            } else
            {
                // linearly interpolate between precalculated points
                absolutePosition.position += (shape.points[index + 1].position - absolutePosition.position).normalized * (positionOnRoad - index) * CONSTANTS.DISTANCE_UNIT;
                absolutePosition.forward = (absolutePosition.forward * (positionOnRoad - index) + shape.points[index + 1].forward * (1 - (positionOnRoad - index))).normalized;
            }

            // set offset to the right to accommodate different lanes
            var perpendicularOffset = (((this.outgoingLanes.Count + other.outgoingLanes.Count) / 2) - this.outgoingLanes.Count + 0.5f + lane) * CONSTANTS.LANE_WIDTH;

            // calculate backwards vector to rotate to right facing vector using Vector2.Perpendicular()
            var inverse = absolutePosition.forward * -1;

            absolutePosition.position += Vector2.Perpendicular(inverse) * perpendicularOffset;

            return absolutePosition;
        }
    }
}
