using UnityEngine;
namespace DataTypes
{ 
    public class Car
    {
        public Edge road;
        public float positionOnRoad;
        public float lane;
        public float speed = 5f; // Laengeneinheiten pro Zeiteinheit
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
            carTransform.position = road.GetPosition(positionOnRoad, lane);
        }

        public void Accelerate(float acceleration)
        {
            speed += acceleration;
        }
    }
}