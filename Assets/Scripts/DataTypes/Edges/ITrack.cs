using System.Collections.Generic;

namespace DataTypes
{
    public interface ITrack
    {
        float length { get; }
        float preferredSpeed { get; }
        List<Car> cars { get; }
        RoadPoint GetAbsolutePosition(float positionOnRoad, float lane);
    }
}