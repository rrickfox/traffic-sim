using Utility;

namespace DataTypes
{
    public interface ITrack
    {
        float length { get; }
        float speedLimit { get; }
        IndexableDictionary<Car> cars { get; }
        RoadPoint GetAbsolutePosition(float positionOnRoad, float lane);
    }
}