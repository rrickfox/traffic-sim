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
                var frontDistance = midpointFrontDistance - averageLength;
                var criticalDistance = Formulas.BrakingDistance(myCar.speed, 0.5 * myCar.maxBrakingDeceleration);

                Acceleration computedAcceleration;
                if (frontDistance <= criticalDistance)
                {
                    if (frontCar.acceleration.MetersPerSecondSquared <= 0)
                    {
                        computedAcceleration = Formulas.BrakingDeceleration(myCar.speed, frontDistance);
                    }
                    else if (myCar.speed > frontCar.speed)
                    {
                        computedAcceleration = Formulas.BrakingDeceleration(myCar.speed, frontDistance);
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
    }
}