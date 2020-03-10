using UnityEngine;

namespace DataTypes
{
    // saves absolute position and forward vector of a point on a bezier curve
    public struct RoadPoint
    {
        public Vector2 position;
        public Vector2 forward;

        public RoadPoint(Vector2 position, Vector2 forward)
        {
            this.position = position;
            this.forward = forward;
        }

        // inverts the forward vector
        public RoadPoint Invert() => new RoadPoint(position, - forward);
    }
}