using UnityEngine;

namespace Utility
{
    public static class CONSTANTS
    {
        // in seconds
        public static readonly float TIME_UNIT = Time.fixedDeltaTime;
        // Distance a car travels at 5kmph in TIME_STEP seconds
        // in unity distance units
        public static readonly float DISTANCE_UNIT = Time.fixedDeltaTime;
        // width of a lane
        public static readonly float LANE_WIDTH = 2.5f;
        // number of points calculated on a bezier curve
        public static readonly float BEZIER_RESOLUTION = 0.001f;
        // height of road Objects
        public static readonly float ROAD_HEIGHT = 0.05f;
    }
}