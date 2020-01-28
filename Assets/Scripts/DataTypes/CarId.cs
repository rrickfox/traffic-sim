namespace DataTypes
{
    public class CarId
    {
        private static int _id = 0;

        public static int id {get => _id++;}
    }
}