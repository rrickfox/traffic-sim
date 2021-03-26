using UnityEngine;
using UnitsNet;
using Utility;

namespace DataTypes
{
    public class SectionTrack : ITrack
    {
        public Vertex vertex; // the Vertex this track is on
        public SortableLinkedList<Car> cars { get; } = new SortableLinkedList<Car>(new CarComparer());
        public RoadShape shape { get; }
        public Length length => shape.length;
        // TODO: calculate accordingly
        public Speed speedLimit { get; } = Speed.FromKilometersPerHour(50);
        public int newLane { get; }

        public TrafficLight light { get; set; }

        public SectionTrack(Vertex vertex, RoadShape shape, int newLane)
        {
            this.vertex = vertex;
            this.shape = shape;
            this.newLane = newLane;
        }

        public RoadPoint GetAbsolutePosition(Length positionOnRoad, float lane = 0)
        {
            // get first estimation of position from saved array of points
            var pos = Mathf.Clamp(positionOnRoad.ToDistanceUnits(), 0, length.ToDistanceUnits());
            var index = Mathf.RoundToInt(pos);
            return shape.points[index];
        }
    }
}