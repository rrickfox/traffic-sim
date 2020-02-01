using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    // represents what you can tell about a road if you were to stand at one of its endpoints
    public class Edge
    {
        // represents how the road would look like from its other endpoint
        public Edge other { get; private set; }
        
        // the Vertex from which this edge originates
        public Vertex vertex = null;
        
        // the cars on the outgoing side of the road
        public List<Car> cars = new List<Car>();
        
        private RoadShape _shape;
        // the shape of the road is synchronized between the two RoadViews
        public RoadShape shape
        {
            get => _shape;
            set => _shape = other._shape = value;
        }

        private float _length;
        // the length of the road is synchronized between the two RoadViews
        public float length
        {
            get => _length;
            // the length gets recalculated automatically and is therefore private
            private set => _length = other._length = value;
        }

        private Vector2 _position;
        // the coordinates of the end of the road from which you look at the road
        public Vector2 position
        {
            get => _position;
            set
            {
                _position = value;
                // automatically update the length upon changing one of the endpoint's position
                length = Vector2.Distance(_position, other._position) / CONSTANTS.DISTANCE_UNIT;
            }
        }
        
        public float angle => Vector2.SignedAngle(other.position - position, Vector2.right);
        
        public List<Lane> outgoingLanes;
        // the incomingLanes of this are just the outgoingLanes of the other view
        public List<Lane> incomingLanes
        {
            get => other.outgoingLanes;
            set => other.outgoingLanes = value;
        }

        public Edge(RoadShape shape, Vector2 position, Vector2 otherPosition,
            List<Lane> outgoingLanes, List<Lane> incomingLanes)
        {
            other = new Edge(this, otherPosition, incomingLanes);
            this.shape = shape;
            this.position = position;
            this.outgoingLanes = outgoingLanes;
        }

        // only construct a RoadView with the values that are unique to this view
        // (the other RoadView needs to take care of the shared values)
        private Edge(Edge other, Vector2 position, List<Lane> outgoingLanes)
        {
            this.other = other;
            _position = position;
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
