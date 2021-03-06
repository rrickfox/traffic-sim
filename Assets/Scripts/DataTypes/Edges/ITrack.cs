using JetBrains.Annotations;
using UnitsNet;
using Utility;

namespace DataTypes
{
    public interface ITrack
    {
        [CanBeNull] TrafficLight light { get; set; }
        Length length { get; }
        Speed speedLimit { get; }
        SortableLinkedList<Car> cars { get; }
        RoadPoint GetAbsolutePosition(Length positionOnRoad, float lane);
    }
}