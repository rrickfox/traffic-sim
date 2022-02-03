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
        private float _curvature;

        public RoadPoint(Vector2 position, Vector2 forward, float curvature)
        {
            this.position = position;
            this.forward = forward;
            this._curvature = curvature;
            this.speedLimit = Speed.FromMetersPerSecond(Mathf.Sqrt(9.81f * 0.8f * 1/_curvature));
        }

        // inverts the forward vector
        public RoadPoint Invert() => new RoadPoint(position, - forward, _curvature);
    }
}