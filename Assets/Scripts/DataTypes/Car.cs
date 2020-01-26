namespace DataTypes
{
    public enum CarState{free, reaktion, action};

    public class Car
    {
        public readonly int id;
        public Edge road;
        public float positionOnRoad;
        public float lane;
        // true if from start to finish
        public Direction direction;
        public CarState state;

        public Car(int id, Edge road, float positionOnRoad, float lane, Direction direction)
        {
            this.id = id;
            this.road = road;
            this.positionOnRoad = positionOnRoad;
            this.lane = lane;
            this.direction = direction;
        }

        public float speed = 0f; // Längeneinheiten pro Zeiteinheit
        
        public void Move()
        {
            positionOnRoad += speed;
            switch(state)
            {
            case CarState.free:
                {
                    
                }
                case CarState.reaction:
                {
                    
                }
                case CarState.action:
                {
                
                }

            }
        }
        public void Accelerate(float acceleation)
        {
            speed += acceleation;
        }
    }
}