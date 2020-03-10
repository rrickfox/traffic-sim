using UnitsNet;

namespace DataTypes.Drivers
{
    public interface IDriver
    {
        Acceleration GetAcceleration();
    }
}