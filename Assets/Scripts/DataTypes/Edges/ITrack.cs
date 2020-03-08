using Utility;

namespace DataTypes
{
    public interface ITrack
    {
        float length { get; }
        float speedLimit { get; }
        SortableLinkedList<Car> cars { get; }
        RoadPoint GetAbsolutePosition(float positionOnRoad, float lane);
    }
}