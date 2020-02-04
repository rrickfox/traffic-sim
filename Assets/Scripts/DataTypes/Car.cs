using UnityEngine;
using Utility;

namespace DataTypes
{ 
    public class Car
    {
        public Edge road { get; private set; }
        public float positionOnRoad { get; private set; }
        public float lane { get; private set; }
        public float speed { get; private set; } = Conversion.UnitsPerTimeStepFromKPH(50); // Laengeneinheiten pro Zeiteinheit
        public Transform carTransform;

        public Car(Edge road, float positionOnRoad, float lane)
        {
            this.road = road;
            this.road.cars.Add(this);
            this.positionOnRoad = positionOnRoad;
            this.lane = lane;
        }
       
        public void Move()
        {
            positionOnRoad += speed;
            var roadPoint = road.GetAbsolutePosition(positionOnRoad, lane);
            carTransform.position = new Vector3(roadPoint.position.x, carTransform.position.y, roadPoint.position.y);
            carTransform.rotation = Quaternion.Euler(0, Vector2.SignedAngle(roadPoint.forward, Vector2.right), 0);
        }

        public void Accelerate(float acceleration)
        {
            speed += acceleration;
        }
    }
}