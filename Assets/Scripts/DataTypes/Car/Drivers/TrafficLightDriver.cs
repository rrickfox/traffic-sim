using UnitsNet;
using Utility;
using Random = UnityEngine.Random;

namespace DataTypes.Drivers
{
    public static class TrafficLightDriver
    {
        public static Acceleration LightAcceleration(Car myCar)
        {
            var acceleration = SimulateHumanness(myCar);

            if (myCar.track.light != null)
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
                                , myCar.track.length - myCar.positionOnRoad - myCar.length)
                            > myCar.maxBrakingDeceleration)
                        {
                            acceleration += myCar.maxAcceleration;
                            break;
                        }
                        acceleration = Formulas.BrakingDeceleration(myCar.speed
                            , myCar.track.length - myCar.positionOnRoad - myCar.length);
                        break;
                    }
                    case TrafficLight.LightState.Red:
                    {
                        acceleration = Formulas.BrakingDeceleration(myCar.speed
                            , myCar.track.length - myCar.positionOnRoad - myCar.length);
                        break;
                    }
                }
            }
            acceleration += myCar.maxAcceleration;
            return acceleration;
        }

        // Add an aspect of randomness to the car's behaviour
        private static Acceleration SimulateHumanness(Car myCar)
            => Random.value * 1000000 < 1
                ? - Acceleration.FromMetersPerSecondSquared(myCar.speed.MetersPerSecond / 3)
                : Acceleration.Zero;
    }
}