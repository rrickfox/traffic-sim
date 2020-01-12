public class Conversion
{
    private CONSTANTS _constants = new CONSTANTS();

    // convert kilometers per hour to units per Timestep
    public float UnitsPerTimeStepFromKPH(float kilometersPerHour)
    {
        return kilometersPerHour * (1000/3600f) * _constants.TIME_STEP;
    }

    // convert meters per second to units per Timestep
    public float UnitsPerTimeStepFromMPS(float metersPerSecond)
    {
        return metersPerSecond * _constants.TIME_STEP;
    }

    // convert units per Timestep to kilometers per hour
    public float KilometersPerHourFromUPTS(float unitsPerTimeStep)
    {
        return (_constants.DISTANCE_STEP / _constants.TIME_STEP) * (3600/1000f);
    }

    // convert units per Timestep to meters per second
    public float MetersPerSecondFromUPTS(float unitsPerTimeStep)
    {
        return _constants.DISTANCE_STEP / _constants.TIME_STEP;
    }
}