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
                var criticalDistance = Formulas.BrakingDistance(myCar.speed, 0.2 * myCar.maxAcceleration);

                Acceleration computedAcceleration;
                if (frontDistance <= criticalDistance)
                {
                    if (frontCar.acceleration.MetersPerSecondSquared <= 0)
                    {
                        // todo what if the frontCat is very far away and myCar has a low speed
                        computedAcceleration = Formulas.BrakingDeceleration(myCar.speed, frontDistance);
                    }
                    else if (myCar.speed > frontCar.speed)
                    {
                        // the minimal distance that is to be kept between this car and the next one
                        var minimumDistance = 1.5 * (myCar.speed.Squared() - frontCar.speed.Squared())
                                              .DividedBy(myCar.maxBrakingDeceleration)
                                              + myCar.bufferDistance;

                        computedAcceleration = frontCar.acceleration -
                                               2 * (myCar.speed + frontCar.speed).Squared().DividedBy(frontDistance)
                                                 * (minimumDistance / frontDistance);
                    }
                    else
                    {
                        computedAcceleration = frontCar.acceleration;
                    }
                }
                else if (myCar.acceleration < frontCar.acceleration)
                {
                    computedAcceleration = frontCar.acceleration;
                }
                else
                {
                    computedAcceleration = myCar.acceleration;
                }
                
                acceleration += Formulas.Min(computedAcceleration, myCar.maxAcceleration);
            }

            return acceleration;
        }

        // Add an aspect of randomness to the car's behaviour
        private static Acceleration SimulateHumanness(Car myCar)
            => Random.value * 1000000 < 1
                ? -Acceleration.FromMetersPerSecondSquared(myCar.speed.MetersPerSecond / 3)
                : Acceleration.Zero;
    }
}