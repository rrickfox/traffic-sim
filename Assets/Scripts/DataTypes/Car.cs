using UnityEngine;
using Utility;
using System.Collections.Generic;

namespace DataTypes
{
    public class Car : GameObjectData<Car, CarBehaviour>
    {
        public ITrack track => segment.track;
        public float positionOnRoad { get; private set; }
        public float lane { get; private set; }
        public float speed { get; private set; } = Conversion.UnitsPerTimeStepFromKPH(50); // Laengeneinheiten pro Zeiteinheit
        public List<RouteSegment> route { get; private set;}
        public RouteSegment segment { get; private set;}

        public Car(GameObject prefab, float lane, List<RouteSegment> route) : base(prefab)
        {
            this.route = route;
            segment = route.PopAt(0);
            track.cars.Add(this);
            positionOnRoad = 0;
            this.lane = lane;

            SetPosition();
        }

        public void Move()
        {
            positionOnRoad += speed;
            SetPosition();
            if(positionOnRoad >= track.length && route.Count > 0)
            {
                positionOnRoad -= track.length;
                track.cars.Remove(this);
                segment = route.PopAt(0);
                track.cars.Add(this);
            }
        }

        private void SetPosition()
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

    public class CarBehaviour : LinkedBehaviour<Car>
    {
        private void FixedUpdate()
        {
            _data.Move();
        }
    }
}