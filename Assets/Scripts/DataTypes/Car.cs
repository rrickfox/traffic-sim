using System.Collections.Generic;
using Events;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;
using Random = UnityEngine.Random;

namespace DataTypes
{
    public class Car : GameObjectData
    {
        public ITrack track => segment.track;
        public List<RouteSegment> route { get; private set; }
        public RouteSegment segment { get; private set; }
        public float positionOnRoad { get; private set; }
        public float lane { get; private set; }
        private int i = 0;
        public float speed { get; private set; } = Conversion.UnitsPerTimeStepFromKPH(Random.value*50 + 30); // Laengeneinheiten pro Zeiteinheit

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
            var stoppingDistance = Distance();

            var frontCar = GetFrontCar();
            
            if (frontCar != null)
            {
                var frontDistance = frontCar.positionOnRoad - positionOnRoad;

                //accelerate
                if (frontDistance >= stoppingDistance && speed < segment.track.preferredSpeed)
                    Accelerate(0.05f);

                //slow down
                if (frontDistance < stoppingDistance)
                    Accelerate(-1);
            }
            else
            {
                //accelerate
                if (speed < segment.track.preferredSpeed)
                {
                    Accelerate(0.05f);
                }
            }

            Human();  

            Move();
        }

        // Returns the stopping distance
        public float Distance()
        {
            return 20 + speed * 20 ;
        }

        // Returns the Car in front of the Car 
        public Car GetFrontCar()
        {
            Car merke = null;
            foreach(var _car in segment.track.cars)
                if (positionOnRoad < _car.positionOnRoad && lane == _car.lane && (merke == null || merke.positionOnRoad > _car.positionOnRoad))
                    merke = _car;
            return merke;  
        }

        public void Human()
        {
            if (Random.value * 1000000 < 1)
                Accelerate(-speed / 3);
        }

        private void Move()
        {           
            positionOnRoad += speed;
            var roadPoint = segment.track.GetAbsolutePosition(positionOnRoad, lane);
            transform.position = new Vector3(roadPoint.position.x, transform.localScale.y / 2 + ROAD_HEIGHT, roadPoint.position.y);

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
            transform.position = new Vector3(roadPoint.position.x, transform.localScale.y / 2 + ROAD_HEIGHT, roadPoint.position.y);
            transform.rotation = Quaternion.Euler(0, Vector2.SignedAngle(roadPoint.forward, Vector2.right), 0);
        }

        // Acclelerate in Units per Timestep
        public void Accelerate(float acceleration)
        {
            speed += acceleration;
        }
    }
}