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

        // converts 2D vector to flat 3D vector (ignoring/setting y-axis)
        public static Vector3 toWorld(this Vector2 source, float y = 0)
            => new Vector3(source.x, y, source.y);

        public static float det(this Vector2 a, Vector2 b)
            => a.x * b.y - a.y * b.x;

        // calculates intersection of two rays, starting at a and b, in the directions u and v respectively
        public static Vector2 intersection(Vector2 a, Vector2 u, Vector2 b, Vector2 v)
        {
            var t = (b - a).det(-1 * v) / u.det(-1 * v);
            return a + t * u;
        }
    }
} 