using System;
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
        
        public ISortableListNode previous { get; set; }
        public ISortableListNode next { get; set; }
        
        public ITrack track => segment.track;
        public List<RouteSegment> route { get; }
        public RouteSegment segment { get; private set; }
        
        // https://de.wikipedia.org/wiki/Gr%C3%B6%C3%9Fenordnung_(Beschleunigung)
        public Acceleration maxAcceleration { get; } = Acceleration.FromMetersPerSecondSquared(4f);
        public Acceleration maxBrakingDeceleration { get; } = - Acceleration.FromMetersPerSecondSquared(10f);
        public Length bufferDistance { get; } = Length.FromMeters(2);

        public Length positionOnRoad { get; private set; } = Length.Zero;
        public float lane { get; private set; } = 0;
        public Speed speed { get; private set; } = Speed.FromMetersPerSecond(30 + Random.value*50);
        public Acceleration acceleration { get; set; } = Acceleration.Zero;
        public IDriver driver { get; private set; }

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
            SelectDriver();
            acceleration = driver.GetAcceleration();
            ExecuteMove();
        }

        private void SelectDriver()
        {
            // TODO: figure out what state the car is in
            driver = new NormalDriver(this, GetFrontCar());
        }

        // Returns the Car in front of the current Car
        private Car GetFrontCar()
            => track.cars.AllGreater(this).FirstOrDefault(other => other.lane == lane);

        private void ExecuteMove()
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
}