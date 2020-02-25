using UnityEngine;
using Utility;
using System.Collections.Generic;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public class Car : GameObjectData<Car, CarBehaviour>
    {
        public Edge road { get; private set; }
        public float positionOnRoad { get; private set; }
        public float lane { get; private set; }
        public float speed { get; private set; } = Conversion.UnitsPerTimeStepFromKPH(70); // Laengeneinheiten pro Zeiteinheit

        public List<Car> otherCars;

        private int i = 0;
        public Car(GameObject prefab, Edge road, float positionOnRoad, float lane) : base(prefab)
        {
            this.road = road;
            this.road.cars.Add(this);
            this.positionOnRoad = positionOnRoad;
            this.lane = lane;

        }
        
        // retrieves position and forward vector of car on road when given relative position on road and lane
        public RoadPoint GetAbsolutePosition()
        {
            // get first estimation of position from saved array of points
            positionOnRoad = Mathf.Clamp(positionOnRoad, 0, road.length);
            var index = Mathf.RoundToInt(positionOnRoad);
            var absolutePosition = road.shape.points[index];

            // set offset to the right to accommodate different lanes
            var perpendicularOffset = lane * (LANE_WIDTH + LINE_WIDTH) + 0.5f * LANE_WIDTH + MIDDLE_LINE_WIDTH / 2f;

            // calculate backwards vector to rotate to right facing vector using Vector2.Perpendicular()
            var inverse = absolutePosition.forward * -1;

            absolutePosition.position += Vector2.Perpendicular(inverse) * perpendicularOffset;

            return absolutePosition;
        }

        public void CarControler()
        {
            var stoppingDistance = Distance();

            Car frontCar = GetFrontCar(stoppingDistance);
            float frontDistance = frontCar.positionOnRoad - positionOnRoad;

            //accelerate
            if (frontCar.speed >= speed || frontCar.speed == -1)
            {
                if (frontDistance > 0 && speed < road.preferedSpeed)
                {
                    Accelerate(0.1f);
                }
            }
            //slow down
            else
            {
                if (frontDistance > 3)
                {
                    Accelerate(-speed / 100);
                }

                if (frontDistance <= 3 && frontDistance > 0)
                {
                    Accelerate(-speed);
                }
            }

            Human();  

            Move();
        }

        // Returns the stopping distance
        public float Distance()
        {
            return MathUtils.Square(speed) +2;
        }

        // Returns the Car in front of the Car 
        public Car GetFrontCar(float distance)
        {
            Car merke = null; 
            foreach(var _car in road.cars)
            { 
                if(positionOnRoad < _car.positionOnRoad && lane == _car.lane)
                {
                    if (merke == null) {merke = _car; }
                    if (merke.positionOnRoad < _car.positionOnRoad) { merke = _car; }
                }
            } 
            return merke;  
        }
      

        public void Human()
        {
            
        }

        private void Move()
        {           
            positionOnRoad += speed;
            var roadPoint = GetAbsolutePosition();
            transform.position = new Vector3(roadPoint.position.x, transform.localScale.y / 2 + CONSTANTS.ROAD_HEIGHT, roadPoint.position.y);
            transform.rotation = Quaternion.Euler(0, Vector2.SignedAngle(roadPoint.forward, Vector2.right), 0);
        }

        // Acclelerate in Units per Timestep
        public void Accelerate(float acceleration)
        {
            speed += acceleration;
        }
    }

    public class CarBehaviour : LinkedBehaviour<Car>
    {
        private void FixedUpdate()
        {
            _data.CarControler();
        }
    }
}