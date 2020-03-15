using UnitsNet;
using Utility;

namespace DataTypes.Drivers
{
    public static class Randomness
    {
        // Add an aspect of randomness to the car's behaviour
        public static Acceleration SimulateHumanness(Car myCar, double chance = .1, double factor = 2)
        {
            var alreadyTooSlow = myCar.speed < .5 * myCar.track.speedLimit;
            if (alreadyTooSlow || Random.RANDOM.NextDouble() > chance)
                return Acceleration.Zero;
            
            var lowerBound = Acceleration.FromMetersPerSecondSquared(- factor * myCar.speed.MetersPerSecond);
            return Random.RANDOM.NextDouble() * lowerBound;
        }
    }
}