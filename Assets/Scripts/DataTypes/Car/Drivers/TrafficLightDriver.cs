using UnitsNet;
using static Utility.Formulas;

namespace DataTypes.Drivers
{
    public static class TrafficLightDriver
    {
        public static Acceleration LightAcceleration(Car myCar)
        {
            var acceleration = Randomness.SimulateHumanness(myCar);
            
            var distanceLeft = myCar.track.length - myCar.positionOnRoad - myCar.criticalBufferDistance;
            var brakingDeceleration = BrakingDeceleration(myCar.speed, distanceLeft);
            
            switch (myCar.track.light.state)
            {
                case TrafficLight.LightState.Green:
                    acceleration += myCar.maxAcceleration;
                    break;
                    
                case TrafficLight.LightState.Yellow:
                    // if (distanceLeft < myCar.finalDistance)
                    //     acceleration += myCar.maxAcceleration;
                    /*else*/
                    if (distanceLeft <= myCar.criticalDistance)
                        acceleration += brakingDeceleration;
                    else
                        acceleration += myCar.maxAcceleration;
                    break;
                    
                case TrafficLight.LightState.Red:
                    if (distanceLeft <= myCar.criticalDistance)
                        acceleration += brakingDeceleration;
                    else
                        acceleration += myCar.maxAcceleration;
                    break;
            }
            
            return Max(myCar.minBrakingDeceleration, Min(myCar.maxAcceleration, acceleration));
        }
    }
}