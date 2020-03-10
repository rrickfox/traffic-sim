using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public class SectionTrack : ITrack
    {
        public Vertex vertex; // the Vertex this track is on
        public List<Car> cars { get; } = new List<Car>();
        public RoadShape shape { get; }
        public float length => shape.length;

        public SectionTrack(Vertex vertex, RoadShape shape)
        {
            this.vertex = vertex;
            this.shape = shape;
        }

        public RoadPoint GetAbsolutePosition(float positionOnRoad, float lane = 0)
        {
            // get first estimation of position from saved array of points
            positionOnRoad = Mathf.Clamp(positionOnRoad, 0, length);
            var index = Mathf.RoundToInt(positionOnRoad);
            return shape.points[index];
        }
    }
}