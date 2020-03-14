using UnitsNet;
using Utility;
using Random = UnityEngine.Random;

namespace DataTypes.Drivers
{
    public static class TrafficLightDriver
    {
        public static Acceleration LightAcceleration(Car myCar)
        {
            var acceleration = Acceleration.Zero;

            if (myCar.track.light != null)
            {
                var brakingDeceleration = Formulas.BrakingDeceleration(myCar.speed,
                    myCar.track.length - CONSTANTS.SECTION_BUFFER_LENGTH.DistanceUnitsToLength() 
                                       - myCar.positionOnRoad - myCar.length / 2);
                
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
                        if (brakingDeceleration * 2 > myCar.maxBrakingDeceleration)
                        {
                            acceleration = brakingDeceleration;
                        }
                        break;
                    }
                }
            }
            return acceleration;
        }
    }
}