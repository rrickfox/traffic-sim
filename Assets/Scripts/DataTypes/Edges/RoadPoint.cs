using UnitsNet;
using UnityEngine;

namespace DataTypes
{
    // saves absolute position and forward vector of a point on a bezier curve
    public struct RoadPoint
    {
        public Vector2 position;
        public Vector2 forward;
        public Speed speedLimit;
        public float curvature;

        public RoadPoint(Vector2 position, Vector2 forward, float curvature)
        {
            this.position = position;
            this.forward = forward;
            this.curvature = curvature;
            // Speed is sqrt(g * coefficientOfFriction * radius), coefficient of friction is on paved, clean, dry road
            this.speedLimit = (curvature != 0) ? Speed.FromMetersPerSecond(Mathf.Sqrt(9.81f * 0.8f * 1/ Mathf.Abs(this.curvature))) : Speed.MaxValue;
        }

        // inverts the forward vector
        public RoadPoint Invert() => new RoadPoint(position, - forward, curvature);
    }
}