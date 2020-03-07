using System;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;
using Random = UnityEngine.Random;

namespace DataTypes
{
    public class Car : GameObjectData, IComparable<Car>
    {
        public ITrack track => segment.track;
        public List<RouteSegment> route { get; private set; }
        public RouteSegment segment { get; private set; }
        
        public float positionOnRoad { get; private set; }
        public float lane { get; private set; }
        public float speed { get; private set; } = Conversion.UnitsPerTimeStepFromKPH(Random.value*50 + 30); // Laengeneinheiten pro Zeiteinheit
        
        private CarState _state { get; set; }
        private enum CarState { DriveNormally, WantToChangeLane }
        
        public static TypePublisher typePublisher { get; } = new TypePublisher();

        public Car(GameObject prefab, float lane, List<RouteSegment> route) : base(prefab)
        {
            this.route = route;
            segment = route.PopAt(0);
            track.cars.Add(this);
            this.lane = lane;
            
            UpdatePosition();
            
            // subscribe to updates
            _publisher = new ObjectPublisher(typePublisher);
            _publisher.Subscribe(CarController);
        }

        public void CarController()
        {
            // TODO: figure out what state the car is in
            _state = CarState.DriveNormally;
            
            switch (_state)
            {
                case CarState.DriveNormally: DriveNormally(); break;
                case CarState.WantToChangeLane: throw new NotImplementedException();
            }
            
            Move();
        }

        private void DriveNormally()
        {
            var stoppingDistance = GetStoppingDistance();

            var frontCar = GetFrontCar();
            
            if (frontCar != null)
            {
                var frontDistance = frontCar.positionOnRoad - positionOnRoad;

                // accelerate
                if (frontDistance >= stoppingDistance && speed < track.speedLimit)
                    Accelerate(0.05f);

                // slow down
                if (frontDistance < stoppingDistance)
                    Accelerate(-1);
            }
            else
            {
                // accelerate
                if (speed < track.speedLimit)
                {
                    Accelerate(0.05f);
                }
            }

            Human();
        }
        
        private float GetStoppingDistance() => 20 + speed * 20;
        
        // Accelerate in Units per Timestep
        private void Accelerate(float acceleration) => speed += acceleration;

        // Returns the Car in front of the current Car 
        private Car GetFrontCar()
        {
            Car merke = null;
            foreach(var _car in track.cars)
                if (positionOnRoad < _car.positionOnRoad && lane == _car.lane && (merke == null || merke.positionOnRoad > _car.positionOnRoad))
                    merke = _car;
            return merke;  
        }

        private void Human()
        {
            if (Random.value * 1000000 < 1)
                Accelerate(-speed / 3);
        }

        private void Move()
        {
            positionOnRoad += speed;
            var roadPoint = track.GetAbsolutePosition(positionOnRoad, lane);
            transform.position = new Vector3(roadPoint.position.x, transform.localScale.y / 2 + ROAD_HEIGHT, roadPoint.position.y);

            UpdatePosition();
            // if car is at end of RouteSegment, get next routeSegment if there is one
            if(positionOnRoad >= track.length && route.Count > 0)
            {
                positionOnRoad -= track.length; // add overshot distance to new RouteSegment
                track.cars.UnsafeRemove(this);
                segment = route.PopAt(0);
                track.cars.Add(this);
            }
        }

        private void UpdatePosition()
        {
            var roadPoint = track.GetAbsolutePosition(positionOnRoad, lane);
            transform.position = new Vector3(roadPoint.position.x, transform.localScale.y / 2 + ROAD_HEIGHT, roadPoint.position.y);
            transform.rotation = Quaternion.Euler(0, Vector2.SignedAngle(roadPoint.forward, Vector2.right), 0);
        }

        public int CompareTo(Car other) => positionOnRoad.CompareTo(other.positionOnRoad);
    }
}