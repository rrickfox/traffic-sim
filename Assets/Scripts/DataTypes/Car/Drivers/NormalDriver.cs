using System;
using UnitsNet;
using Utility;
using Random = UnityEngine.Random;

namespace DataTypes.Drivers
{
    public struct NormalDriver : IDriver
    {
        private Car _myCar { get; }
        private Car _frontCar { get; }
        private Acceleration _acceleration { get; set; }

        public NormalDriver(Car myCar, Car frontCar)
        {
            _myCar = myCar;
            _frontCar = frontCar;
            _acceleration = Acceleration.Zero;
        }

        public Acceleration GetAcceleration()
        {
            SimulateHumanness();
            
            if (_frontCar == null)
                return _myCar.maxAcceleration;

            // TODO
            if (_frontCar.positionOnRoad == _myCar.positionOnRoad)
                return Random.value * _myCar.maxAcceleration;

            var minimumDistance = GetMinimumDistance();
            var collisionTime = GetCollisionTime();
            var frontDistance = _frontCar.positionOnRoad - _myCar.positionOnRoad;
            var computedAcceleration = _frontCar.acceleration
                                       + 2 * (frontDistance - minimumDistance) / collisionTime / collisionTime
                                       + 2 * (_frontCar.speed - _myCar.speed) / collisionTime;
            return Formulas.Min(computedAcceleration, _myCar.maxAcceleration);
        }

        // The Time to Collision
        private TimeSpan GetCollisionTime()
            => (_frontCar.positionOnRoad - _myCar.positionOnRoad - CONSTANTS.CAR_LENGTH).DividedBy(_myCar.speed - _frontCar.speed);

        private Length GetMinimumDistance()
            => 1.5 * (_myCar.speed.Squared() - _frontCar.speed.Squared()).DividedBy(_myCar.maxAcceleration);

        // Add an aspect of randomness to the car's behaviour
        private void SimulateHumanness()
        {
            if (Random.value * 1000000 < 1)
                _acceleration -= Acceleration.FromMetersPerSecondSquared(_myCar.speed.MetersPerSecond / 3);
        }
    }
}