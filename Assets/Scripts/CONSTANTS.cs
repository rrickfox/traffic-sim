using UnityEngine;
public class CONSTANTS
{
    //in seconds
    public float TIME_STEP = Time.fixedDeltaTime;
    //Distance a car travels at 5kmph in TIME_STEP seconds
    //in unity distance units
    public float DISTANCE_STEP = 5 * (1000/3600f) * Time.fixedDeltaTime;
}
