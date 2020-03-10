using UnitsNet;

namespace DataTypes.Drivers
{
    public class NullDriver : IDriver
    {
        public Acceleration acceleration { get; } = Acceleration.Zero;
    }
}