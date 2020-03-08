using System.Collections.Generic;

namespace DataTypes
{
    public class CarComparer : IComparer<Car>
    {
        public int Compare(Car x, Car y)
            => x.positionOnRoad.CompareTo(y.positionOnRoad);
    }
}