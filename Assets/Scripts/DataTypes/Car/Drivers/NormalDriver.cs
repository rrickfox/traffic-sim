using UnitsNet;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace DataTypes.Drivers
{
    public static class NormalDriver
    {
        public static Acceleration NormalAcceleration(Car myCar, Car frontCar)
        {
            var acceleration = SimulateHumanness(myCar);

            if (frontCar == null)
                acceleration += myCar.maxAcceleration;
            else
            {
                var midpointFrontDistance = frontCar.positionOnRoad - myCar.positionOnRoad;
                var averageLength = (myCar.length + frontCar.length) / 2;
                if (midpointFrontDistance < averageLength)
                {
                    // TODO: handle this better
                    Debug.LogWarning("Cars are crashing into each other");
                    acceleration += Random.value * myCar.maxAcceleration;
                }
                else
                {
                    var frontDistance = midpointFrontDistance - averageLength;
                    // the minimal distance that is to be kept between this car and the next one
                    var minimumDistance = 1.5 * (myCar.speed.Squared() - frontCar.speed.Squared()).DividedBy(myCar.maxBrakingDeceleration)
                                                 + frontDistance + myCar.bufferDistance;
                    var computedAcceleration = frontCar.acceleration
                                               - 2 * (myCar.speed - frontCar.speed).Squared()
                                               .DividedBy(frontDistance)
                                               .Times(minimumDistance + Length.FromMeters(1.5))
                                               .DividedBy(frontDistance);
                    // ensure that the acceleration does not exceed the maximum acceleration
                    acceleration += Formulas.Min(computedAcceleration, myCar.maxAcceleration);
                }
            }

            return acceleration;
        }

        // Add an aspect of randomness to the car's behaviour
        private static Acceleration SimulateHumanness(Car myCar)
            => Random.value * 1000000 < 1
                ? - Acceleration.FromMetersPerSecondSquared(myCar.speed.MetersPerSecond / 3)
                : Acceleration.Zero;
    }
}