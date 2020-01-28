namespace DataTypes
{
    public class Car
    {
        public readonly int id;
        public Edge road;
        public float positionOnRoad;
        public float lane;
        public Direction direction;

        public Car(int id, Edge road, float positionOnRoad, float lane, Direction direction)
        {
            this.id = id;
            this.road = road;
            this.positionOnRoad = positionOnRoad;
            this.lane = lane;
            this.direction = direction;
        }
    }
}