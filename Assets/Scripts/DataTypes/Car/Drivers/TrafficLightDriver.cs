using UnitsNet;
using Utility;
using Random = UnityEngine.Random;

namespace DataTypes.Drivers
{
    public static class TrafficLightDriver
    {
        public static Acceleration LightAcceleration(Car myCar)
        {
           // var acceleration = SimulateHumanness(myCar);
            var acceleration = Acceleration.Zero;

            if (myCar.track.light != null)
            {
                var brakingDeceleration = Formulas.BrakingDeceleration(myCar.speed,
                    myCar.track.length - myCar.positionOnRoad + myCar.length / 2 -
                    CONSTANTS.SECTION_BUFFER_LENGTH.DistanceUnitsToLength());
                
                switch (myCar.track.light.state)
                {
                    case TrafficLight.LightState.Green:
                    {
                        acceleration += myCar.maxAcceleration;
                        break;
                    }
                    case TrafficLight.LightState.Yellow:
                    {
                        if (brakingDeceleration > myCar.maxBrakingDeceleration)
                        {
                            acceleration += myCar.maxAcceleration;
                            break;
                        }
                        acceleration = brakingDeceleration;
                        break;
                    }
                    case TrafficLight.LightState.Red:
                    {
                        acceleration = brakingDeceleration;
                        break;
                    }
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