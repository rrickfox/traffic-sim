using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;
using Random = UnityEngine.Random;

namespace DataTypes
{
    public class Car : GameObjectData, ISortableListNode
    {
        private static float _ACCELERATION { get; } = 0.05f;
        private static float _DEACCELERATION { get; } = -1f;
        
        public static TypePublisher typePublisher { get; } = new TypePublisher();
        
        public ISortableListNode previous { get; set; }
        public ISortableListNode next { get; set; }

        public ITrack track => segment.track;
        public List<RouteSegment> route { get; }
        public RouteSegment segment { get; private set; }
        
        public float positionOnRoad { get; private set; }
        public float lane { get; private set; }
        public float speed { get; private set; } = Conversion.UnitsPerTimeStepFromKPH(Random.value*50 + 30); // Laengeneinheiten pro Zeiteinheit
        
        private CarState _state { get; set; }
        private enum CarState { DriveNormally, WantToChangeLane }

        public Car(GameObject prefab, float lane, List<RouteSegment> route) : base(prefab)
        {
            this.route = route;
            segment = route.PopAt(0);
            track.cars.AddFirst(this);
            this.lane = lane;

            // give car a random color
            gameObject.GetComponent<MeshRenderer>().material.color = UnityEngine.Random.ColorHSV();

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
            var stoppingDistance = GetStoppingDistance();

            var frontCar = GetFrontCar();
            
            if (frontCar != null)
            {
                var frontDistance = frontCar.positionOnRoad - positionOnRoad;

                // accelerate
                if (frontDistance >= stoppingDistance && speed < track.speedLimit)
                    Accelerate(_ACCELERATION);

                // slow down
                if (frontDistance < stoppingDistance)
                    Accelerate(_DEACCELERATION);
            }
            else
            {
                // accelerate
                if (speed < track.speedLimit)
                    Accelerate(_ACCELERATION);
            }

            SimulateHumanness();
        }
        
        private float GetStoppingDistance() => 20 + speed * 20;
        
        // Accelerate in Units per Timestep
        private void Accelerate(float acceleration) => speed += acceleration;

        // Returns the Car in front of the current Car
        private Car GetFrontCar()
        {
            return track.cars.AllGreater(this).FirstOrDefault(other => other.lane == lane);
        }

        // Add an aspect of randomness to the car's behaviour
        private void SimulateHumanness()
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