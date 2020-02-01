using UnityEngine;

namespace DataTypes
{
    public struct RoadPoint
    {
        public Vector2 position;
        public Vector2 forward;

        public RoadPoint(Vector2 position, Vector2 forward)
        {
            this.position = position;
            this.forward = forward;
        }
    }
}