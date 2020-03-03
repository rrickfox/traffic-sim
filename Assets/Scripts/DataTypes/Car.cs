using UnityEngine;
using Utility;
using System.Collections.Generic;

namespace DataTypes
{
    public class Car : GameObjectData<Car, CarBehaviour>
    {
        public ITrack track;
        public float positionOnRoad { get; private set; } = 0;
        public float lane { get; private set; }
        public float speed { get; private set; } = Conversion.UnitsPerTimeStepFromKPH(50); // Laengeneinheiten pro Zeiteinheit
        public List<RouteSegment> route { get; private set;}
        public RouteSegment segment { get; private set;}

        public Car(GameObject prefab, float lane, List<RouteSegment> route) : base(prefab)
        {
            this.route = route;
            segment = route.PopAt(0);
            track = segment.edge;
            track.cars.Add(this);
            this.lane = lane;

            UpdatePosition();
        }

        public void Move()
        {
            positionOnRoad += speed;
            UpdatePosition();
            // if car is at end of RouteSegment, get next routeSegment if there is one
            if(positionOnRoad >= track.length && route.Count > 0)
            {
                positionOnRoad -= track.length; // add overshot distance to new RouteSegment
                if(track is SectionTrack)
                {
                    track.cars.Remove(this);
                    segment = route.PopAt(0);
                    track = segment.edge;
                    track.cars.Add(this);
                } else if (track is Edge) // could also be standard else
                {
                    track.cars.Remove(this);
                    try
                    {
                        track = segment.edge.other.vertex.routes[segment][(int) lane];
                    } catch(KeyNotFoundException e)
                    {
                        Dispose();
                        throw new System.Exception("Car tried to take route it cannot reach.", e);
                    }
                    track.cars.Add(this);
                }
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

    public class CarBehaviour : LinkedBehaviour<Car>
    {
        private void FixedUpdate()
        {
            _data.Move();
        }
    }
}