using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using UnitsNet;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;
using Random = UnityEngine.Random;

namespace DataTypes
{
    public class Car : GameObjectData, ISortableListNode
    {
        public static TypePublisher typePublisher { get; } = new TypePublisher();
        
        public ISortableListNode previous { get; set; }
        public ISortableListNode next { get; set; }
        
        public ITrack track => segment.track;
        public List<RouteSegment> route { get; }
        public RouteSegment segment { get; private set; }
        
        // https://de.wikipedia.org/wiki/Gr%C3%B6%C3%9Fenordnung_(Beschleunigung)
        public Acceleration maxAcceleration { get; } = Acceleration.FromMetersPerSecondSquared(4f);
        public Acceleration maxBrakingDeceleration { get; } = - Acceleration.FromMetersPerSecondSquared(10f);
        public Length bufferDistance { get; } = Length.FromMeters(50f);

        public Length positionOnRoad { get; private set; } = Length.Zero;
        public float lane { get; private set; }
        public Speed speed { get; private set; } = Speed.FromMetersPerSecond(30 + Random.value*50);
        public Acceleration acceleration { get; set; } = Acceleration.Zero;

        private CarState _state { get; set; }
        private enum CarState { DriveNormally, WantToChangeLane }

        public Car(GameObject prefab, float lane, List<RouteSegment> route) : base(prefab)
        {
            this.route = route;
            segment = route.PopAt(0);
            track.cars.AddFirst(this);
            this.lane = lane;

            // give car a random color
            gameObject.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();

            UpdatePosition();
            
            // subscribe to updates
            _publisher = new ObjectPublisher(typePublisher);
            _publisher.Subscribe(Drive);
        }

        private void Drive()
        {
            DetermineState();
            var driver = SelectDriver();
            acceleration = driver.GetAcceleration();
            ExecuteAcceleration();
        }

        private void DetermineState()
        {
            // TODO: figure out what state the car is in
            _state = CarState.DriveNormally;
        }

        private IDriver SelectDriver()
        {
            switch (_state)
            {
                case CarState.DriveNormally:
                    return new NormalCarState(this, GetFrontCar());
                case CarState.WantToChangeLane:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Returns the Car in front of the current Car
        private Car GetFrontCar()
            => track.cars.AllGreater(this).FirstOrDefault(other => other.lane == lane);

        private void ExecuteAcceleration()
        {
            speed += acceleration.Times(TimeSpan.FromSeconds(1));
            positionOnRoad += speed * TimeSpan.FromSeconds(1);
            
            UpdatePosition();
            
            // if car is at end of RouteSegment, get next routeSegment if there is one
            if(positionOnRoad >= track.length && route.Count > 0)
            {
                positionOnRoad -= track.length; // add overshot distance to new RouteSegment
                track.cars.Remove(this);
                segment = route.PopAt(0);
                track.cars.AddFirst(this);
            }
        }

        private void UpdatePosition()
        {
            var roadPoint = track.GetAbsolutePosition(positionOnRoad, lane);
            transform.position = new Vector3(roadPoint.position.x, transform.localScale.y / 2 + ROAD_HEIGHT, roadPoint.position.y);
            transform.rotation = Quaternion.Euler(0, Vector2.SignedAngle(roadPoint.forward, Vector2.right), 0);
        }
    }

    public interface IDriver
    {
        Acceleration GetAcceleration();
    }
    
    public struct NormalCarState : IDriver
    {
        private Car _myCar { get; }
        private Car _frontCar { get; }
        private Acceleration _acceleration { get; set; }

        public NormalCarState(Car myCar, Car frontCar)
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
            => (_frontCar.positionOnRoad - _myCar.positionOnRoad - CAR_LENGTH).DividedBy(_myCar.speed - _frontCar.speed);

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