public class Conversion
{
    private CONSTANTS Constants = new CONSTANTS();

    //convert kilometers per hour to units per Timestep
    public float UnitPerTimeStepFromKPH(float KilometersPerHour)
    {
        return KilometersPerHour * (1000/3600f) * Constants.TIME_STEP;
    }

    //convert meters pr second to units per Timestep
    public float UnitPerTimeStepFromMPS(float MetersPerSecond)
    {
        return MetersPerSecond * Constants.TIME_STEP;
    }

    //convert units per Timestep to kilometers per hour
    public float KilometersPerHourFromUPTS(float UnitPerTimeStep)
    {
        return (Constants.DISTANCE_STEP / Constants.TIME_STEP) * (3600/1000f);
    }

    //convert units per Timestep to meters per second
    public float MetersPerSecondFromUPTS(float UnitPerTimeStep)
    {
        return Constants.DISTANCE_STEP / Constants.TIME_STEP;
    }
}