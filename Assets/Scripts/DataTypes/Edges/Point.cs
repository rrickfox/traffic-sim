using UnityEngine;

namespace DataTypes
{
    public class Point
    {
        public Vector2 position;

        public Point(Vector2 position)
        {
            this.position = position;
        }

        public Point(Point point)
        {
            position = point.position;
        }
    }
}