using UnityEngine;
namespace DataTypes
{ 
    public class Car
    {
        public Edge road;
        public float positionOnRoad;
        public float lane;
        public float speed = Conversion.UnitsPerTimeStepFromKPH(50); // Laengeneinheiten pro Zeiteinheit
        public Transform carTransform;

        public Car(Edge road, float positionOnRoad, float lane)
        {
            this.road = road;
            this.positionOnRoad = positionOnRoad;
            this.lane = lane;
        }
       
        public void Move()
        {
            positionOnRoad += speed;
            Vector2 position = road.GetPosition(positionOnRoad, lane);
            carTransform.position = new Vector3(position.x, carTransform.position.y, position.y);
        }

        public void Accelerate(float acceleration)
        {
            speed += acceleration;
        }
    }
}