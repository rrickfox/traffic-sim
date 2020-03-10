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
        // https://de.wikipedia.org/wiki/Gr%C3%B6%C3%9Fenordnung_(Beschleunigung)
        private static Acceleration _MAX_ACCELERATION { get; } = Acceleration.FromMetersPerSecondSquared(4f);
        private static Acceleration _MAX_BRAKING_DECELERATION { get; } = - Acceleration.FromMetersPerSecondSquared(10f);
        private static Length _BUFFER_DISTANCE { get; } = Length.FromMeters(50f);
        
        public static TypePublisher typePublisher { get; } = new TypePublisher();
        
        public ISortableListNode previous { get; set; }
        public ISortableListNode next { get; set; }

        public ITrack track => segment.track;
        public List<RouteSegment> route { get; }
        public RouteSegment segment { get; private set; }

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

        public void Drive()
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
            var frontCar = GetFrontCar();
            SimulateHumanness();
            Accelerate(GetAcceleration(frontCar));
        }
        
        // Accelerate in Units per Timestep
        private void Accelerate(Acceleration acceleration)
            => speed += acceleration.Times(TimeSpan.FromSeconds(1));

        // Returns the Car in front of the current Car
        private Car GetFrontCar()
        {
            return track.cars.AllGreater(this).FirstOrDefault(other => other.lane == lane);
        }

        //Returns the Time to Collision
        private TimeSpan GetCollisionTime(Car frontCar)
            => (frontCar.positionOnRoad - positionOnRoad - CAR_LENGTH).DividedBy(speed - frontCar.speed);

        private Length GetMinimumDistance(Car frontCar)
            => 3/2 * (speed.Squared() - frontCar.speed.Squared()).DividedBy(_MAX_ACCELERATION);

        private Acceleration GetAcceleration(Car frontCar)
        {
            if (frontCar == null)
                return _MAX_ACCELERATION;

            // TODO
            if (frontCar.positionOnRoad == positionOnRoad)
                return Random.value * _MAX_ACCELERATION;

            var minimumDistance = GetMinimumDistance(frontCar);
            var collisionTime = GetCollisionTime(frontCar);
            var frontDistance = frontCar.positionOnRoad - positionOnRoad;
            var computedAcceleration = frontCar.acceleration
                + 2 * (frontDistance - minimumDistance) / collisionTime / collisionTime
                + 2 * (frontCar.speed - speed) / collisionTime;
            return Formulas.Min(computedAcceleration, _MAX_ACCELERATION);
        }

        // Add an aspect of randomness to the car's behaviour
        private void SimulateHumanness()
        {
            if (Random.value * 1000000 < 1)
                Accelerate(Acceleration.FromMetersPerSecondSquared(-speed.MetersPerSecond / 3));
        }

        private void Move()
        {
            positionOnRoad += speed * TimeSpan.FromSeconds(1);
            var roadPoint = track.GetAbsolutePosition(positionOnRoad, lane);
            transform.position = new Vector3(roadPoint.position.x, transform.localScale.y / 2 + ROAD_HEIGHT, roadPoint.position.y);

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
}