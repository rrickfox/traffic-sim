namespace Utility
{
    public static class Conversion
    {
        // convert kilometers per hour to units per Timestep
        public static float UnitsPerTimeStepFromKPH(float kilometersPerHour)
            => UnitsPerTimeStepFromMPS(kilometersPerHour * (10/36f));

        // convert meters per second to units per Timestep
        public static float UnitsPerTimeStepFromMPS(float metersPerSecond)
            => metersPerSecond;

        // convert units per Timestep to kilometers per hour
        public static float KilometersPerHourFromUPTU(float unitsPerTimeUnit)
            => MetersPerSecondFromUPTU(unitsPerTimeUnit) * 3.6f;

        // convert units per Timestep to meters per second
        public static float MetersPerSecondFromUPTU(float unitsPerTimeUnit)
            => unitsPerTimeUnit;

        // convert meters per second^2 to units per Timestep^2
        public static float UnitsPerTimeStepFromMPS2(float metersPerSecond2)
            => UnitsPerTimeStepFromMPS(metersPerSecond2) / CONSTANTS.TIME_UNIT;
    }
}