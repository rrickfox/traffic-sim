using UnityEngine;
using System.Collections.Generic;

namespace DataTypes
{
    public class Edge : RoadView
    {
        // allows this derived class to treat _other as an instance of Edge and not just RoadView
        public new Edge other { get => (Edge)_other; set => _other = value; }
        // the Vertex from which this edge originates
        public Vertex vertex;

        public Edge(RoadView view, Vertex vertex, RoadView otherView, Vertex otherVertex) : base(view)
        {
            other = new Edge(otherView, otherVertex, this);
            this.vertex = vertex;
        }

        private Edge(RoadView view, Vertex vertex, Edge otherEdge) : base(view)
        {
            other = otherEdge;
            this.vertex = vertex;
        }

        public List<Car> cars = new List<Car>();

        public Vector2 GetPosition(float positionOnRoad, float lane)
        {
            Vector2 position = Vector2.Lerp(this.position, other.position, positionOnRoad);
            // set offset to the right to accomodate different lanes
            float offset = (((this.outgoingLanes.Count + other.outgoingLanes.Count) / 2) - this.outgoingLanes.Count + 0.5f + lane) * CONSTANTS.LANE_WIDTH;

            // calculate backwards vector to rotate to right facing vector using Vector2.Perpendicular()
            Vector2 backward = (other.position - this.position).normalized * -1;

            position += Vector2.Perpendicular(backward) * offset;

            return position;
        }
    }
}