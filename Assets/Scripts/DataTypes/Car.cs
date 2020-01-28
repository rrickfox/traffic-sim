namespace DataTypes
{
    public class Car
    {
        public Edge road;
        public float positionOnRoad;
        public float lane;

        public Car(Edge road, float positionOnRoad, float lane)
        {
            this.road = road;
            this.positionOnRoad = positionOnRoad;
            this.lane = lane;
        }
    }
}