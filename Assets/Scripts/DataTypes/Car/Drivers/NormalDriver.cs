using UnitsNet;
using Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DataTypes.Drivers
{
    public static class NormalDriver
    {
        private static int delta = 4; // magic number, see https://en.wikipedia.org/wiki/Intelligent_driver_model
        public static Acceleration NormalAcceleration(Car myCar, Car frontCar)
        {
            var acceleration = SimulateHumanness(myCar);
            acceleration += freeRoadBehaviour(myCar);

            if (frontCar != null)
            {
                // position is at middle of car, get distance between midpoints and remove half of each car length
                var frontDistance = (frontCar.positionOnRoad - myCar.positionOnRoad) - ((myCar.length + frontCar.length) / 2);

                acceleration += interactionBehaviour(myCar, frontDistance, frontCar.speed);
            }

            return acceleration;
        }

        public static Acceleration freeRoadBehaviour(Car myCar)
            => myCar.maxAcceleration * (1 - Mathf.Pow((float) (myCar.speed / myCar.track.speedLimit), delta));

        public static Acceleration interactionBehaviour(Car myCar, Length frontDistance, Speed frontSpeed)
        {
            // TODO: change speedlimit for each driver
            var s = Length.FromMeters(
                myCar.bufferDistance.Meters //TODO: variate depending on speed
                + (myCar.speed * (frontDistance / myCar.track.speedLimit)).Meters // Minimum time the gap could be breached
                + (myCar.speed.MetersPerSecond * (myCar.speed.MetersPerSecond - frontSpeed.MetersPerSecond))
                    / (2 * Mathf.Sqrt(((float) myCar.maxAcceleration.MetersPerSecondSquared) * (-1f * (float) myCar.brakingDeceleration.MetersPerSecondSquared)))
            );

            return -1 * myCar.maxAcceleration * (s / frontDistance) * (s / frontDistance);
        }

        // Add an aspect of randomness to the car's behaviour
        private static Acceleration SimulateHumanness(Car myCar)
            => Random.value * 1000000 < 1
                ? -Acceleration.FromMetersPerSecondSquared(myCar.speed.MetersPerSecond / 3)
                : Acceleration.Zero;
    }
}