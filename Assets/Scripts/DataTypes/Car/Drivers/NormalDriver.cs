using System;
using UnitsNet;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace DataTypes.Drivers
{
    public struct NormalDriver : IDriver
    {
        private Car _myCar { get; }
        private Car _frontCar { get; }
        public Acceleration acceleration { get; private set; }

        public NormalDriver(Car myCar, Car frontCar)
        {
            _myCar = myCar;
            _frontCar = frontCar;
            acceleration = Acceleration.Zero;

            InitializeAcceleration();
        }

        private void InitializeAcceleration()
        {
            SimulateHumanness();

            if (_frontCar == null)
                acceleration = _myCar.maxAcceleration;

            // TODO
            else if (_frontCar.positionOnRoad - _myCar.positionOnRoad < (_myCar.length + _frontCar.length) / 2)
            {
                Debug.LogWarning("Cars are crashing into each other");
                acceleration =  Random.value * _myCar.maxAcceleration;
            }

            else
            {
                var minimumDistance = GetMinimumDistance();
                var collisionTime = GetCollisionTime();
                var frontDistance = _frontCar.positionOnRoad - _myCar.positionOnRoad;
                var computedAcceleration = _frontCar.acceleration
                                           + 2 * (frontDistance - minimumDistance) / collisionTime / collisionTime
                                           + 2 * (_frontCar.speed - _myCar.speed) / collisionTime;
                // ensure that the acceleration does not exceed the maximum acceleration
                var minAcceleration = Formulas.Min(computedAcceleration, _myCar.maxAcceleration);
                // ensure that the acceleration is not below zero
                acceleration = Formulas.Max(minAcceleration, Acceleration.Zero);
            }
        }

        // The Time to Collision
        private TimeSpan GetCollisionTime()
        {
            if (_myCar.speed - _frontCar.speed < Speed.FromMillimetersPerSecond(1))
                return TimeSpan.MaxValue;
            
            return (_frontCar.positionOnRoad - _myCar.positionOnRoad - _myCar.length)
                .DividedBy(_myCar.speed - _frontCar.speed);
        }

        // Get the minimal distance that is to be kept between this car and the next one
        private Length GetMinimumDistance()
            => 1.5 * (_myCar.speed.Squared() - _frontCar.speed.Squared()).DividedBy(_myCar.maxAcceleration) + _myCar.bufferDistance;

        // Add an aspect of randomness to the car's behaviour
        private void SimulateHumanness()
        {
            if (Random.value * 1000000 < 1)
                acceleration -= Acceleration.FromMetersPerSecondSquared(_myCar.speed.MetersPerSecond / 3);
        }
    }
}