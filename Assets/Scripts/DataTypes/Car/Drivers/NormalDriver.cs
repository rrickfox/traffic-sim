using System;
using System.Linq;
using UnitsNet;
using Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DataTypes.Drivers
{
    public static class NormalDriver
    {
        private static int delta = 4; // magic number, see https://en.wikipedia.org/wiki/Intelligent_driver_model
        public static Acceleration NormalAcceleration(Car myCar, Car frontCar, Length position)
        {
            var acceleration = SimulateHumanness(myCar);
            acceleration += freeRoadBehaviour(myCar);

            var frontDistance = Length.MaxValue;

            if (frontCar != null)
            {
                // position is at middle of car, get distance between midpoints and remove half of each car length
                frontDistance = (position - myCar.positionOnRoad) - ((myCar.length + frontCar.length) / 2);

                acceleration += interactionBehaviour(myCar, frontDistance, frontCar.speed);
            }

            acceleration += slowDownBehaviour(myCar, frontDistance);

            // at least brake so the speed of the curve is met, if needed slow down even more by computed Acceleration
            return acceleration; // CheckNeedToSlowDown(myCar)
        }

        public static Acceleration freeRoadBehaviour(Car myCar)
            // => myCar.maxAcceleration * (1 - Mathf.Pow((float) (myCar.speed / myCar.track.GetSpeedLimitAtPosition(myCar.positionOnRoad)), delta)); // same Error as with slowDownBehaviour
            => myCar.maxAcceleration * (1 - Mathf.Pow((float) (myCar.speed / myCar.track.speedLimit), delta));

        public static Acceleration interactionBehaviour(Car myCar, Length frontDistance, Speed frontSpeed)
        {
            // TODO: change speedlimit for each driver
            var s = Length.FromMeters(
                myCar.bufferDistance.Meters
                // + (myCar.speed * GetMinimumTimeToReachPosition(myCar, frontDistance)).Meters // same Error as with slowDownBehaviour
                + (myCar.speed * (frontDistance / myCar.track.speedLimit)).Meters // Minimum time the gap could be breached
                + (myCar.speed.MetersPerSecond * (myCar.speed.MetersPerSecond - frontSpeed.MetersPerSecond))
                    / (2 * Mathf.Sqrt(((float) myCar.maxAcceleration.MetersPerSecondSquared) * ((float) myCar.brakingDeceleration.Inverse().MetersPerSecondSquared)))
            );

            return -1 * myCar.maxAcceleration * (s / frontDistance) * (s / frontDistance);
        }

        public static Acceleration slowDownBehaviour(Car myCar, Length frontDistance)
        {
            // sadly this is still not refined enough, so this will remain unused until further development
            return Acceleration.Zero;

            // var lookAheadDistance = Formulas.BrakingDistance(myCar.speed, myCar.brakingDeceleration.Inverse()) * 2;
            // var points = myCar.track.GetRoadPointsInRange(myCar.positionOnRoad, lookAheadDistance).ToList();
            // var length = points.Count() * 1f.DistanceUnitsToLength();

            // if (length < lookAheadDistance)
            // {
            //     var nextTrack = myCar.GetNextTrack();
            //     if (nextTrack != null)
            //         points.AddRange(nextTrack.GetRoadPointsInRange(Length.Zero, lookAheadDistance - length));
            // }

            // Speed limit = points.Min(point => point.speedLimit);
            // Length distanceToSpeedDecrease = points.FindIndex(point => point.speedLimit == limit) * 1f.DistanceUnitsToLength();

            // if (myCar.speed > limit && distanceToSpeedDecrease < frontDistance) // only possible if speedLimit is reduced
            //     return interactionBehaviour(myCar, distanceToSpeedDecrease, limit);

            // return Acceleration.Zero;
        }

        private static TimeSpan GetMinimumTimeToReachPosition(Car myCar, Length distance)
        {
            Length goal = myCar.positionOnRoad + distance;
            Length actual = myCar.positionOnRoad;
            TimeSpan dur = TimeSpan.Zero;
            ITrack track = myCar.track;
            while(actual < goal) {
                actual += track.GetSpeedLimitAtPosition(actual).Times(1f.TimeUnitsToTimeSpan());
                dur += 1f.TimeUnitsToTimeSpan();
                if (actual > track.length)
                {
                    goal -= track.length;
                    actual -= track.length;
                    track = myCar.GetNextTrack();
                }
                if (track == null)
                    return dur;
            }
            return dur;
        }

        // Add an aspect of randomness to the car's behaviour
        private static Acceleration SimulateHumanness(Car myCar)
            => Random.value * 1000000 < 1
                ? -Acceleration.FromMetersPerSecondSquared(myCar.speed.MetersPerSecond / 3)
                : Acceleration.Zero;
    }
}