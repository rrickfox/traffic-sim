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
                var distanceToLight = myCar.track.length
                    - CONSTANTS.SECTION_BUFFER_LENGTH.DistanceUnitsToLength()
                    - myCar.positionOnRoad
                    - myCar.length / 2;
                
                switch (myCar.track.light.state)
                {
                    case TrafficLight.LightState.Green:
                    {
                        acceleration = NormalDriver.freeRoadBehaviour(myCar);
                        break;
                    }
                    case TrafficLight.LightState.Yellow:
                    {
                        if (Formulas.BrakingDistance(myCar.speed, myCar.brakingDeceleration * -1) > distanceToLight)
                        {
                            acceleration = NormalDriver.freeRoadBehaviour(myCar);
                            break;
                        }
                        acceleration = NormalDriver.freeRoadBehaviour(myCar) + NormalDriver.interactionBehaviour(myCar, distanceToLight, Speed.Zero);
                        break;
                    }
                    case TrafficLight.LightState.Red:
                    {
                        acceleration = NormalDriver.freeRoadBehaviour(myCar) + NormalDriver.interactionBehaviour(myCar, distanceToLight, Speed.Zero);
                        break;
                    }
                }
            }
            return acceleration;
        }
    }
}