using System.Collections.Generic;

namespace DataTypes
{
    public interface ITrack
    {
        float length { get; }
        float speedLimit { get; }
        List<Car> cars { get; }
        RoadPoint GetAbsolutePosition(float positionOnRoad, float lane);
    }
}