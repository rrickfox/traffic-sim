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
            
            switch (myCar.track.light.states[myCar.segment.laneType])
            {
                case TrafficLight.LightState.Green:
                    acceleration += NormalDriver.freeRoadBehaviour(myCar);
                    break;
                    
                case TrafficLight.LightState.Yellow:
                    // if (distanceLeft < myCar.finalDistance)
                    //     acceleration += myCar.maxAcceleration;
                    /*else*/
                    if (distanceLeft <= myCar.criticalDistance)
                        acceleration += NormalDriver.freeRoadBehaviour(myCar) + NormalDriver.interactionBehaviour(myCar, distanceLeft, Speed.Zero) + NormalDriver.slowDownBehaviour(myCar, distanceLeft);
                    else
                        acceleration += NormalDriver.freeRoadBehaviour(myCar) + NormalDriver.slowDownBehaviour(myCar, Length.MaxValue);
                    break;
                    
                case TrafficLight.LightState.Red:
                    if (distanceLeft <= myCar.criticalDistance)
                        acceleration += NormalDriver.freeRoadBehaviour(myCar) + NormalDriver.interactionBehaviour(myCar, distanceLeft, Speed.Zero) + NormalDriver.slowDownBehaviour(myCar, distanceLeft);
                    else
                        acceleration += NormalDriver.freeRoadBehaviour(myCar) + NormalDriver.slowDownBehaviour(myCar, Length.MaxValue);
                    break;
            }
            
            return Max(myCar.brakingDeceleration, Min(myCar.maxAcceleration, acceleration));
        }
    }
}