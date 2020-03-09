using System.Collections.Generic;
using JetBrains.Annotations;

namespace DataTypes
{
    public interface ITrack
    {
        float length { get; }
        List<Car> cars { get; }
        // needed for access by cars
        [CanBeNull] TrafficLight light { get; set; }
        RoadPoint GetAbsolutePosition(float positionOnRoad, float lane);
    }
}