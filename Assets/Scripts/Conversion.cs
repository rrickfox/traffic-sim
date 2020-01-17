public class Conversion
{
    // convert kilometers per hour to units per Timestep
    public float UnitsPerTimeStepFromKPH(float kilometersPerHour)
    {
        return UnitsPerTimeStepFromMPS(kilometersPerHour * (10/36f));
    }

    // convert meters per second to units per Timestep
    public float UnitsPerTimeStepFromMPS(float metersPerSecond)
    {
        return metersPerSecond;
    }

    // convert units per Timestep to kilometers per hour
    public float KilometersPerHourFromUPTU(float unitsPerTimeUnit)
    {
        return MetersPerSecondFromUPTU(unitsPerTimeUnit) * 3.6f;
    }

    // convert units per Timestep to meters per second
    public float MetersPerSecondFromUPTU(float unitsPerTimeUnit)
    {
        return unitsPerTimeUnit;
    }
}