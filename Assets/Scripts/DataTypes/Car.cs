using UnityEngine;

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

            var position = GetAbsolutePosition();
            transform.position = new Vector3(position.x, CONSTANTS.ROAD_HEIGHT + prefab.transform.localScale.y / 2, position.y);
            transform.rotation = Quaternion.Euler(0, road.angle, 0);
        }
        
        public Vector2 GetAbsolutePosition()
        {
            var result = Vector2.Lerp(road.position, road.other.position, positionOnRoad / road.length);
            // set offset to the right to accomodate different lanes
            var perpandicularOffset = ((road.outgoingLanes.Count + road.incomingLanes.Count) / 2 - road.outgoingLanes.Count + 0.5f + lane) * CONSTANTS.LANE_WIDTH;

            // calculate backwards vector to rotate to right facing vector using Vector2.Perpendicular()
            var inverse = (road.other.position - road.position).normalized * -1;

            result += Vector2.Perpendicular(inverse) * perpandicularOffset;

            return result;
        }

        public void Move()
        {
            positionOnRoad += speed;
            var position = GetAbsolutePosition();
            transform.position = new Vector3(position.x, transform.position.y, position.y);
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