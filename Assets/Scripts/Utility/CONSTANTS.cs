using UnityEngine;
using UnitsNet;

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
        public static readonly float LANE_WIDTH = 3f;
        // number of points calculated on a bezier curve
        public static readonly float BEZIER_RESOLUTION = 0.001f;
        // height of road Objects
        public static readonly float ROAD_HEIGHT = 0.15f;

        // width of the middle line on the road
        public static readonly float MIDDLE_LINE_WIDTH = 0.2f;
        // width of the standard line
        public static readonly float LINE_WIDTH = 0.2f;
        // width of the line at the border of a road
        public static readonly float BORDER_LINE_WIDTH = 0.3f;
        // ratio of length of space to length of line
        public static readonly float LINE_RATIO = 2f;
        // length of line segment (line + space)
        public static readonly float LINE_LENGTH = 12f;
        // value which determines width of texture
        public static readonly float WIDTH_MULTIPLIER = Mathf.Pow(10, Mathf.Max(
            MathUtils.DecimalPlaces(MIDDLE_LINE_WIDTH), 
            MathUtils.DecimalPlaces(LINE_WIDTH), 
            MathUtils.DecimalPlaces(BORDER_LINE_WIDTH),
            MathUtils.DecimalPlaces(ROAD_HEIGHT), 
            MathUtils.DecimalPlaces(LANE_WIDTH)));
        public static readonly GameObject EMPTY_PREFAB = Resources.Load("Empty", typeof(GameObject)) as GameObject;
        public static readonly GameObject CAR_PREFAB = Resources.Load("Car", typeof(GameObject)) as GameObject;
        public static readonly GameObject ROAD_PREFAB = Resources.Load("Road", typeof(GameObject)) as GameObject;
        // acceleration a car has when driving freely
        public static readonly float STANDARD_ACCELERATION = 0.05f;
        // acceleration with which a car brakes
        public static readonly float BRAKE_ACCELERATION = -1f;
        public static readonly Length CAR_LENGTH = 5f.FromUnityDistanceUnits();
    }
}