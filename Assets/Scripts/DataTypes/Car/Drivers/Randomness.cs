using UnitsNet;
using UnityEngine;

namespace DataTypes.Drivers
{
    public static class Randomness
    {
        // Add an aspect of randomness to the car's behaviour
        public static Acceleration SimulateHumanness(Car myCar)
            => Random.value * 1000000 < 1
                ? -Acceleration.FromMetersPerSecondSquared(myCar.speed.MetersPerSecond / 3)
                : Acceleration.Zero;
    }
}