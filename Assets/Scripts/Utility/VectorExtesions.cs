using UnityEngine;

namespace Utility
{
    public static class VectorExtensions
    {
        // converts 2D vector to flat 3D vector (ingnoring/setting y-axis)
        public static Vector3 toWorld(this Vector2 source, float y = 0)
            => new Vector3(source.x, y, source.y);
    }
}