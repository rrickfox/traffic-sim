using UnityEngine;
public class CONSTANTS
{
    // in seconds
    public readonly float TIME_UNIT = Time.fixedDeltaTime;
    // Distance a car travels at 5kmph in TIME_STEP seconds
    // in unity distance units
    public readonly float DISTANCE_UNIT = 5 * (10/36f) * Time.fixedDeltaTime;
}
