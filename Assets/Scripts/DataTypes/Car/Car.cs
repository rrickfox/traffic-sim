using System.Collections.Generic;
using System.Linq;
using DataTypes.Drivers;
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
        public override GameObject prefab { get; } = CAR_PREFAB;

        public ISortableListNode previous { get; set; }
        public ISortableListNode next { get; set; }
        
        public ITrack track => segment.track;
        public List<RouteSegment> route { get; }
        public RouteSegment segment { get; private set; }
        
        // https://de.wikipedia.org/wiki/Gr%C3%B6%C3%9Fenordnung_(Beschleunigung)
        public Acceleration maxAcceleration { get; } = Acceleration.FromMetersPerSecondSquared(4);
        public Acceleration maxBrakingDeceleration { get; } = Acceleration.FromMetersPerSecondSquared(-111111);
        public Length bufferDistance => length / 2;
        public Length length { get; } = Length.FromMeters(5);

        public Length positionOnRoad { get; private set; } = Length.Zero;
        public float lane { get; private set; } = 0;
        public Speed speed { get; private set; } = Speed.Zero;
        public Acceleration acceleration { get; private set; }

        public Car(float lane, List<RouteSegment> route)
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
            SelectDriver();
            ExecuteMove();
        }

        private void SelectDriver()
        {
            // TODO: figure out what state the car is in
            var frontCar = GetFrontCar();
            acceleration = NormalDriver.NormalAcceleration(this, frontCar);
        }

        // Returns the Car in front of the current Car
        private Car GetFrontCar()
            => track.cars.LookAhead(this).FirstOrDefault(other => other.lane == lane && other.positionOnRoad > positionOnRoad);

        private void ExecuteMove()
        {
            var newSpeed = speed + acceleration.Times(Formulas.TimeUnitsToTimeSpan(1));
            if (newSpeed > track.speedLimit)
            {
                // enforce the speed limit
                speed = track.speedLimit;
                acceleration = Acceleration.Zero;
            }
            else if (newSpeed.MetersPerSecond <= 0)
            {
                // ensure that the car does not drive backwards
                speed = Speed.Zero;
                acceleration = Acceleration.Zero;
            }
            else
            {
                speed = newSpeed;
            }
            
            positionOnRoad += speed * Formulas.TimeUnitsToTimeSpan(1);
            
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