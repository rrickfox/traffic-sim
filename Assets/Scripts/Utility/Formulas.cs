using System;
using System.Linq;
using UnitsNet;

namespace Utility
{
    public static partial class Formulas
    {
        public static float ToTimeUnits(this TimeSpan timeSpan)
            => timeSpan.Seconds * CONSTANTS.TIME_UNIT;

        public static TimeSpan TimeUnitsToTimeSpan(this float timeUnits)
            => TimeSpan.FromSeconds(timeUnits / CONSTANTS.TIME_UNIT);

        public static float ToDistanceUnits(this Length length)
            => (float) length.Meters * CONSTANTS.DISTANCE_UNIT;

        public static Length DistanceUnitsToLength(this float distanceUnits)
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

        public static TimeSpan DividedBy(this Length length, Speed speed)
            => TimeSpan.FromSeconds(length.Meters / speed.MetersPerSecond);

        public static Speed Times(this Acceleration acceleration, TimeSpan timeSpan)
            => Speed.FromMetersPerSecond(acceleration.MetersPerSecondSquared * timeSpan.Seconds);

        public static SpecificEnergy Times(this Acceleration acceleration, Length length)
            => SpecificEnergy.FromJoulesPerKilogram(acceleration.MetersPerSecondSquared * length.Meters);

        public static SpecificEnergy Squared(this Speed speed) => speed * speed;

        public static Area Squared(this Length length) => length * length;

        public static Acceleration Min(params Acceleration[] accelerations) => accelerations.Min();
        
        public static Acceleration Max(params Acceleration[] accelerations) => accelerations.Max();
        
        public static Speed Max(params Speed[] speeds) => speeds.Max();
        
        // https://de.wikipedia.org/wiki/Bremsweg
        public static Length BrakingDistance(Speed speed, Acceleration brakingDeceleration)
            => speed.Squared().DividedBy(2 * brakingDeceleration);

        // https://de.wikipedia.org/wiki/Bremsverz%C3%B6gerung
        public static Acceleration BrakingDeceleration(Speed speed, Length brakingDistance)
            => -speed.Squared().DividedBy(2 * brakingDistance);
    }
}