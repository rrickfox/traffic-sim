using UnityEngine;

namespace Utility
{
    public static class VectorExtensions
    {
        // find point on a line between to points, that is closest to the given point
        public static Vector2 closestPointOnLinesegment(this Vector2 point, Vector2 lineA, Vector2 lineB)
        {
            var dir = (lineB - lineA).normalized;
            var lambda = Vector2.Dot(point - lineA, dir);
            return lineA + lambda * dir;
        }

        // converts 2D vector to flat 3D vector (ingnoring/setting y-axis)
        public static Vector3 toWorld(this Vector2 source, float y = 0)
            => new Vector3(source.x, y, source.y);
    }
} 