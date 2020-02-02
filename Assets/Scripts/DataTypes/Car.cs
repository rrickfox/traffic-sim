using UnityEngine;

namespace DataTypes
{
    public class Car : GameObjectData<Car, CarBehaviour>
    {
        public Edge road { get; private set; }
        public float positionOnRoad { get; private set; }
        public float lane { get; private set; }
        public float speed { get; private set; } = Conversion.UnitsPerTimeStepFromKPH(50); // Laengeneinheiten pro Zeiteinheit

        public Car(GameObject prefab, Edge road, float positionOnRoad, float lane)
        {
            this.road = road;
            this.road.cars.Add(this);
            this.positionOnRoad = positionOnRoad;
            this.lane = lane;
            
            // display the car graphically
            var position = road.GetAbsolutePosition(positionOnRoad, lane);
            CreateGameObject(
                prefab: prefab,
                position: new Vector3(position.x, prefab.transform.localScale.y / 2 + prefab.transform.localScale.y / 2, position.y),
                rotation: Quaternion.Euler(0, road.angle, 0)
            );
            gameObject.name = $"Car({gameObject.GetInstanceID()})";
        }

        public void Move()
        {
            positionOnRoad += speed;
            var position = road.GetAbsolutePosition(positionOnRoad, lane);
            gameObject.transform.position = new Vector3(position.x, gameObject.transform.position.y, position.y);
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