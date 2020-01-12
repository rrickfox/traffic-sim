using UnityEngine;
public class CONSTANTS
{
    //in seconds
    private float _TimeStep = Time.fixedDeltaTime;
    //Distance a car travels at 5kmph in TIME_STEP seconds
    //in unity distance units
    private float _DistanceStep = 5 * (1000/3600f) * Time.fixedDeltaTime;

    public float TIME_STEP
    {
        get {return _TimeStep;}
    }
    public float DISTANCE_STEP
    {
        get {return _DistanceStep;}
    }
}
