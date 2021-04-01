using UnitsNet;
using Utility;

namespace DataTypes.Drivers
{
    public static class NormalDriver
    {
        public static Acceleration NormalAcceleration(Car myCar, Car frontCar)
        {
            var acceleration = Randomness.SimulateHumanness(myCar);

            if (frontCar == null)
            {
                acceleration += myCar.maxAcceleration;
            }
            else
            {
                var midpointFrontDistance = frontCar.positionOnRoad - myCar.positionOnRoad;
                var averageLength = (myCar.length + frontCar.length) / 2;
                var frontDistance = midpointFrontDistance - averageLength - myCar.bufferDistance;

                Acceleration computedAcceleration;
                
                if (frontDistance <= myCar.criticalDistance)
                    if (frontCar.acceleration.MetersPerSecondSquared <= 0 || myCar.speed > frontCar.speed)
                        computedAcceleration = Formulas.BrakingDeceleration(myCar.speed, frontDistance);
                    else
                        computedAcceleration = frontCar.acceleration;
                else
                    if(frontCar.acceleration.MetersPerSecondSquared <= 0.1) // edge case where a car can stand still, because front car is at a red light
                        computedAcceleration = myCar.maxAcceleration;
                    else
                        computedAcceleration = Formulas.Max(myCar.acceleration, frontCar.acceleration);

                acceleration += Formulas.Min(computedAcceleration, myCar.maxAcceleration);
            }

            return acceleration;
        }
    }
}