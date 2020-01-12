public class Conversion
{
    private CONSTANTS _constants = new CONSTANTS();

    // convert kilometers per hour to units per Timestep
    public float UnitsPerTimeStepFromKPH(float kilometersPerHour)
    {
        return UnitsPerTimeStepFromMPS(kilometersPerHour * (10/36f));
    }

    // convert meters per second to units per Timestep
    public float UnitsPerTimeStepFromMPS(float metersPerSecond)
    {
        return metersPerSecond * _constants.TIME_STEP;
    }

    // convert units per Timestep to kilometers per hour
    public float KilometersPerHourFromUPTS(float unitsPerTimeStep)
    {
        return MetersPerSecondFromUPTS(unitsPerTimeStep * 3.6f);
    }

    // convert units per Timestep to meters per second
    public float MetersPerSecondFromUPTS(float unitsPerTimeStep)
    {
        return unitsPerTimeStep * _constants.DISTANCE_STEP / _constants.TIME_STEP;
    }
}