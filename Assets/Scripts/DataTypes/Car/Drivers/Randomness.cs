using UnitsNet;
using Utility;

namespace DataTypes.Drivers
{
    public static class Randomness
    {
        // Add an aspect of randomness to the car's behaviour
        public static Acceleration SimulateHumanness(Car myCar, double chance = .1, double factor = .3)
        {
            if (Random.RANDOM.NextDouble() > chance)
                return Acceleration.Zero;
            
            var lowerBound = Acceleration.FromMetersPerSecondSquared(- factor * myCar.speed.MetersPerSecond);
            return Random.RANDOM.NextDouble() * lowerBound;
        }
    }
}