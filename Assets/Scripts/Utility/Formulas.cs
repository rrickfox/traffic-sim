using System;
using System.Linq;
using UnitsNet;

namespace Utility
{
    public static class Formulas
    {
        public static float ToUnityTimeUnits(this TimeSpan timeSpan)
            => timeSpan.Seconds * CONSTANTS.TIME_UNIT;

        public static TimeSpan FromUnityTimeUnits(this float timeUnits)
            => TimeSpan.FromSeconds(timeUnits / CONSTANTS.TIME_UNIT);

        public static float ToUnityDistanceUnits(this Length length)
            => (float) length.Meters * CONSTANTS.DISTANCE_UNIT;

        public static Length FromUnityDistanceUnits(this float distanceUnits)
            => Length.FromMeters(distanceUnits / CONSTANTS.DISTANCE_UNIT);

        public static Acceleration DividedBy(this Speed speed, TimeSpan timeSpan)
            => Acceleration.FromMetersPerSecondSquared(speed.MetersPerSecond / timeSpan.Seconds);
        
        public static TimeSpan DividedBy(this Speed speed, Acceleration acceleration)
            => TimeSpan.FromSeconds(speed.MetersPerSecond / acceleration.MetersPerSecondSquared);
        
        public static Frequency DividedBy(this Speed speed, Length length)
            => Frequency.FromPerSecond(speed.MetersPerSecond / length.Meters);

        public static Acceleration DividedBy(this SpecificEnergy specificEnergy, Length length)
            => Acceleration.FromMetersPerSecondSquared(specificEnergy.JoulesPerKilogram / length.Meters);

        public static Length DividedBy(this SpecificEnergy specificEnergy, Acceleration acceleration)
            => Length.FromMeters(specificEnergy.JoulesPerKilogram / acceleration.MetersPerSecondSquared);

        public static Speed Times(this Acceleration acceleration, TimeSpan timeSpan)
            => Speed.FromMetersPerSecond(acceleration.MetersPerSecondSquared * timeSpan.Seconds);

        public static SpecificEnergy Squared(this Speed speed) => speed * speed;

        public static Acceleration Min(params Acceleration[] accelerations)
            => accelerations.Min();
        
        // https://de.wikipedia.org/wiki/Bremsweg
        public static Length BrakingDistance(Speed speed, Acceleration brakingDeceleration)
            => speed.Squared().DividedBy(2 * brakingDeceleration);

        // https://de.wikipedia.org/wiki/Bremsverz%C3%B6gerung
        public static Acceleration BrakingDeceleration(Speed speed, Length brakingDistance)
            => -speed.Squared().DividedBy(2 * brakingDistance);
    }
}