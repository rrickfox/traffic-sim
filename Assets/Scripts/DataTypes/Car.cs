using UnityEngine;
using Utility;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public class Car : GameObjectData<Car, CarBehaviour>
    {
        public Edge road { get; private set; }
        public float positionOnRoad { get; private set; }
        public float lane { get; private set; }
        public float speed { get; private set; } = Conversion.UnitsPerTimeStepFromKPH(50); // Laengeneinheiten pro Zeiteinheit

        public Car(GameObject prefab, Edge road, float positionOnRoad, float lane) : base(prefab)
        {
            this.road = road;
            this.road.cars.Add(this);
            this.positionOnRoad = positionOnRoad;
            this.lane = lane;

            SetPosition();
        }
        
        // retrieves position and forward vector of car on road when given relative position on road and lane
        public RoadPoint GetAbsolutePosition()
        {
            // get first estimation of position from saved array of points
            positionOnRoad = Mathf.Clamp(positionOnRoad, 0, road.length);
            var index = Mathf.RoundToInt(positionOnRoad);
            var absolutePosition = road.shape.points[index];

            // set offset to the right to accommodate different lanes
            var perpendicularOffset = (((road.outgoingLanes.Count + road.incomingLanes.Count) / 2) - road.outgoingLanes.Count + 0.5f + lane) * LANE_WIDTH;

            // calculate backwards vector to rotate to right facing vector using Vector2.Perpendicular()
            var inverse = absolutePosition.forward * -1;

            absolutePosition.position += Vector2.Perpendicular(inverse) * perpendicularOffset;

            return absolutePosition;
        }

        public void Move()
        {
            positionOnRoad += speed;
            SetPosition();
        }

        private void SetPosition()
        {
            var roadPoint = GetAbsolutePosition();
            transform.position = new Vector3(roadPoint.position.x, transform.position.y, roadPoint.position.y);
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