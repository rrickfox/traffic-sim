using System.Collections.Generic;

namespace DataTypes
{
    public interface ITrack
    {
        RoadShape shape { get; }
        float length { get; }
        List<Car> cars { get; }
        RoadPoint GetAbsolutePosition(float positionOnRoad, float lane);
    }
}