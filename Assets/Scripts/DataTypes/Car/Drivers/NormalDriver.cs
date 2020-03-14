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
            {
                acceleration += myCar.maxAcceleration;
            }
            else
            {
                var midpointFrontDistance = frontCar.positionOnRoad - myCar.positionOnRoad;
                var averageLength = (myCar.length + frontCar.length) / 2;
                var frontDistance = midpointFrontDistance - averageLength;

                if (frontCar.acceleration.MetersPerSecondSquared <= 0)
                {
                    acceleration = Formulas.BrakingDeceleration(myCar.speed, frontDistance);
                }
                else if (myCar.speed > frontCar.speed)
                {
                    // the minimal distance that is to be kept between this car and the next one
                    var minimumDistance =
                        1.5 * (myCar.speed.Squared() - frontCar.speed.Squared())
                              .DividedBy(myCar.maxBrakingDeceleration)
                        + myCar.bufferDistance;
                    
                    var computedAcceleration = frontCar.acceleration -
                                               2 * (myCar.speed + frontCar.speed).Squared().DividedBy(frontDistance)
                                                 * (minimumDistance / frontDistance);
                    
                    acceleration += Formulas.Min(computedAcceleration, myCar.maxAcceleration);
                }
                else if (myCar.acceleration < frontCar.acceleration)
                {
                    acceleration = frontCar.acceleration;
                }
                else
                {
                    acceleration = myCar.acceleration;
                }
            }

            return acceleration;
        }

        // Get the minimal distance that is to be kept between this car and the next one
        private static Length MinimumDistance(Car myCar, Car frontCar)
            => 1.5 * (myCar.speed.Squared() - frontCar.speed.Squared()).DividedBy(myCar.maxAcceleration) + myCar.bufferDistance;

        // Add an aspect of randomness to the car's behaviour
        private static Acceleration SimulateHumanness(Car myCar)
            => Random.value * 1000000 < 1
                ? - Acceleration.FromMetersPerSecondSquared(myCar.speed.MetersPerSecond / 3)
                : Acceleration.Zero;
    }
}