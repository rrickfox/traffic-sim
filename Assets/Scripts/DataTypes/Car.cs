using UnityEngine;
using Utility;
using System.Collections.Generic;
using Events;

namespace DataTypes
{
    public class Car : GameObjectData
    {
        public ITrack track => segment.track;
        public float positionOnRoad { get; private set; } = 0;
        public float lane { get; private set; }
        public float speed { get; private set; } = Conversion.UnitsPerTimeStepFromKPH(50); // Laengeneinheiten pro Zeiteinheit
        public List<RouteSegment> route { get; private set; }
        public RouteSegment segment { get; private set; }
        
        public static TypePublisher typePublisher { get; } = TypePublisher.Create<Car>();

        public Car(GameObject prefab, float lane, List<RouteSegment> route) : base(prefab)
        {
            this.route = route;
            segment = route.PopAt(0);
            track.cars.Add(this);
            this.lane = lane;

            UpdatePosition();
            
            // subscribe to updates
            _publisher = new ObjectPublisher(typePublisher);
            _publisher.Subscribe(Move);
        }

        public void Move()
        {
            positionOnRoad += speed;
            UpdatePosition();
            // if car is at end of RouteSegment, get next routeSegment if there is one
            if(positionOnRoad >= track.length && route.Count > 0)
            {
                positionOnRoad -= track.length; // add overshot distance to new RouteSegment
                track.cars.Remove(this);
                segment = route.PopAt(0);
                track.cars.Add(this);
            }
        }

        private void UpdatePosition()
        {
            var roadPoint = track.GetAbsolutePosition(positionOnRoad, lane);
            transform.position = new Vector3(roadPoint.position.x, transform.localScale.y / 2 + CONSTANTS.ROAD_HEIGHT, roadPoint.position.y);
            transform.rotation = Quaternion.Euler(0, Vector2.SignedAngle(roadPoint.forward, Vector2.right), 0);
        }

        public void Accelerate(float acceleration)
        {
            speed += acceleration;
        }
    }
}