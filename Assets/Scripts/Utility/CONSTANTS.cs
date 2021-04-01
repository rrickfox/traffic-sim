using UnityEngine;
using System.Collections.Generic;

namespace Utility
{
    public static class CONSTANTS
    {
        // in seconds
        public static readonly float TIME_UNIT = Time.fixedDeltaTime;
        // Distance a car travels at 1m/s in TIME_STEP seconds
        // in unity distance units (equivalent to real world meters)
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
        // value which determines width of texture of road
        public static readonly float WIDTH_MULTIPLIER_ROAD = Mathf.Pow(10, Mathf.Max(
            MathUtils.DecimalPlaces(MIDDLE_LINE_WIDTH), 
            MathUtils.DecimalPlaces(LINE_WIDTH), 
            MathUtils.DecimalPlaces(BORDER_LINE_WIDTH),
            MathUtils.DecimalPlaces(ROAD_HEIGHT), 
            MathUtils.DecimalPlaces(LANE_WIDTH)));
        public static readonly GameObject EMPTY_PREFAB = Resources.Load<GameObject>("Empty");
        public static readonly Dictionary<GameObject, float> CAR_PREFABS = new Dictionary<GameObject, float>(){
            {Resources.Load<GameObject>("Car1"), 4},
            {Resources.Load<GameObject>("Car2"), 8},
            {Resources.Load<GameObject>("Car3"), 8},
            {Resources.Load<GameObject>("Car4"), 4.5f},
            {Resources.Load<GameObject>("Car5"), 4.5f},
            {Resources.Load<GameObject>("Car6"), 4.5f},
            {Resources.Load<GameObject>("Car7"), 4.5f},
            {Resources.Load<GameObject>("Car8"), 4.5f},
            {Resources.Load<GameObject>("Car9"), 3.75f},
            {Resources.Load<GameObject>("Car10"), 4.5f}
        };
        public static readonly GameObject ROAD_PREFAB = Resources.Load<GameObject>("Road");
        // length of buffer between stopping line and intersection
        public static readonly float SECTION_BUFFER_LENGTH = 2f;
        // width of stop line at intersection
        public static readonly float STOP_LINE_WIDTH = 0.4f;
        // value which determines width of texture of crosssection
        public static readonly float MULTIPLIER_SECTION = Mathf.Pow(10, Mathf.Max(
            MathUtils.DecimalPlaces(MIDDLE_LINE_WIDTH), 
            MathUtils.DecimalPlaces(LINE_WIDTH), 
            MathUtils.DecimalPlaces(BORDER_LINE_WIDTH),
            MathUtils.DecimalPlaces(ROAD_HEIGHT), 
            MathUtils.DecimalPlaces(LANE_WIDTH),
            MathUtils.DecimalPlaces(SECTION_BUFFER_LENGTH),
            MathUtils.DecimalPlaces(STOP_LINE_WIDTH)));
        // offset of trafficlight to right side of the road
        public static readonly float TRAFFICLIGHT_OFFSET = 0.5f;
    }
}