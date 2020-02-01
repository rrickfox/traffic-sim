using UnityEngine;

public static class CONSTANTS
{
    // in seconds
    public static readonly float TIME_UNIT = Time.fixedDeltaTime;
    // Distance a car travels at 5kmph in TIME_STEP seconds
    // in unity distance units
    public static readonly float DISTANCE_UNIT = Time.fixedDeltaTime;
    // width of a lane
    public static readonly float LANE_WIDTH = 3f;
    // amount of points calculated per beziercurve
    public static readonly float BEZIER_RESOLUTION = 0.01f;
}
