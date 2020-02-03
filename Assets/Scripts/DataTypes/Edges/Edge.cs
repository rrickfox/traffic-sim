using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    // represents what you can tell about a road if you were to stand at one of its endpoints
    public class Edge : GameObjectData<Edge, EdgeBehaviour>
    {
        // the Vertex from which this edge originates
        public IVertex vertex = null;
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
        public float length { get; }
        public float angle { get; }

        public Edge(GameObject prefab, RoadShape shape, Vector2 position, Vector2 otherPosition,
            List<Lane> outgoingLanes, List<Lane> incomingLanes) : base(prefab)
        {
            this.shape = shape;
            this.position = position;
            this.outgoingLanes = outgoingLanes;
            length = Vector2.Distance(position, otherPosition) / CONSTANTS.DISTANCE_UNIT;
            angle = Vector2.SignedAngle(otherPosition - position, Vector2.right);
            other = new Edge(this, otherPosition, incomingLanes);

            var middlePoint = (other.position - this.position) * 0.5f + this.position;
            transform.position = new Vector3(middlePoint.x, 0, middlePoint.y);
            transform.rotation = Quaternion.Euler(0, Vector2.SignedAngle(other.position - this.position, Vector2.right), 0);
            transform.localScale = new Vector3(
                x: Vector2.Distance(this.position, other.position), // road length
                y: transform.localScale.y, 
                z: (this.outgoingLanes.Count + this.incomingLanes.Count) * CONSTANTS.LANE_WIDTH // road width
            );
        }

        // construct an Edge where other is already constructed
        private Edge(Edge other, Vector2 position, List<Lane> outgoingLanes)
        {
            this.other = other;
            this.position = position;
            this.outgoingLanes = outgoingLanes;
            length = this.other.length;
            angle = this.other.angle;
        }
    }

    public class EdgeBehaviour : LinkedBehaviour<Edge> { }
}
