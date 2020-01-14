namespace DataTypes
{
    public class Car
    {
        public readonly int id;
        public Road road;
        public float positionOnRoad;
        public float lane;
        // true if from start to finish
        public bool direction;

        public Car(int setId, Road setRoad, float setPositionOnRoad, float setLane, bool setDirection)
        {
            id = setId;
            road = setRoad;
            positionOnRoad = setPositionOnRoad;
            lane = setLane;
            direction = setDirection;
        }
    }
}