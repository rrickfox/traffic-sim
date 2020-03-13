using System.Security.Cryptography;
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

            if (myCar.track.light != null && frontCar == null)
            {
                switch (myCar.track.light.state)
                {
                    case TrafficLight.LightState.Green:
                    {
                        acceleration += myCar.maxAcceleration;
                        break;
                    }
                    case TrafficLight.LightState.Yellow:
                    {
                        if (Formulas.BrakingDeceleration(myCar.speed
                                , myCar.track.length - myCar.positionOnRoad)
                            > myCar.maxBrakingDeceleration)
                        {
                            acceleration += myCar.maxAcceleration;
                            break;
                        }
                        acceleration = Formulas.BrakingDeceleration(myCar.speed
                            , myCar.track.length - myCar.positionOnRoad);
                        break;
                    }
                    case TrafficLight.LightState.Red:
                    {
                        acceleration = Formulas.BrakingDeceleration(myCar.speed
                            , myCar.track.length - myCar.positionOnRoad);
                        break;
                    }
                }
            }
            else if (frontCar == null)
            {
                acceleration += myCar.maxAcceleration;
            }
            else
            {
                var midpointFrontDistance = frontCar.positionOnRoad - myCar.positionOnRoad;
                var averageLength = (myCar.length + frontCar.length) / 2;
                if (midpointFrontDistance < averageLength)
                {
                    // TODO: handle this bet
                    Debug.LogWarning("Cars are crashing into each other");
                    acceleration += Random.value * myCar.maxAcceleration;
                }
                else
                {
                    var frontDistance = midpointFrontDistance - averageLength;
                    var minimumDistance = MinimumDistance(myCar, frontCar);
                    var computedAcceleration = frontCar.acceleration
                                               - 2 * (myCar.speed - frontCar.speed).Squared()
                                               .DividedBy(frontDistance)
                                               .Times(minimumDistance + Length.FromMeters(1.5))
                                               .DividedBy(frontDistance);
                    // ensure that the acceleration does not exceed the maximum acceleration
                    var minAcceleration = Formulas.Min(computedAcceleration, myCar.maxAcceleration);
                    // ensure that the acceleration is not below zero
                    acceleration += Formulas.Max(minAcceleration, Acceleration.Zero);
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