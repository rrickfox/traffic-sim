using UnityEngine;
using UnitsNet;
using Utility;
using System.Collections.Generic;

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

        public Speed GetSpeedLimitAtPosition(Length position)
            => Formulas.Min(shape.points[Mathf.RoundToInt((float) (position / 1f.DistanceUnitsToLength()))].speedLimit, speedLimit);

        public RoadPoint GetAbsolutePosition(Length positionOnRoad, float lane = 0)
        {
            // get first estimation of position from saved array of points
            var pos = Mathf.Clamp(positionOnRoad.ToDistanceUnits(), 0, length.ToDistanceUnits());
            var index = Mathf.RoundToInt(pos);
            return shape.points[index];
        }

        public IEnumerable<RoadPoint> GetRoadPointsInRange(Length start, Length range) {
            for(var i = (int) start.ToDistanceUnits(); i < (start + range).ToDistanceUnits() && i < shape.points.Length; i++) {
                yield return shape.points[i];
            }
        }
    }
}