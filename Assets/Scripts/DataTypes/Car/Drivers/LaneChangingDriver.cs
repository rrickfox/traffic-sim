using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnityEngine;
using Utility;

namespace DataTypes.Drivers
{
    public static class LaneChangingDriver
    {
        // returns the updated lane value and an acceleration limit
        public static (float lane, Acceleration maxAcceleration) ChangeLane(Car myCar, Direction laneChangeDirection)
        {
            // the update to lane if it's possible to change lanes
            var plannedChange = myCar.laneChangingRate * (int) laneChangeDirection;
            
            var sideCars = CarsNextTo(myCar, laneChangeDirection, plannedChange).ToList();
            /* TODO: when 2 cars stand next to each other at the end of the road
                     (with 0 speed because the traffic light was red)
                     then they will just stay there
               possible Solution:
                     breakingDistance should be subtracted by the distance it will
                     take for this car to change to the desired lane
            */
            
            if (sideCars.Count > 0)
                // don't change lanes and slow down by at least `maxAcceleration`
                return (
                    lane: myCar.lane,
                    maxAcceleration: Formulas.BrakingDeceleration(myCar.speed,
                        myCar.track.length
                        - CONSTANTS.SECTION_BUFFER_LENGTH.DistanceUnitsToLength()
                        - myCar.positionOnRoad
                        - myCar.length / 2)
                );
            
            // change lanes regularly as no cars are in the way
            return (
                lane: myCar.lane + plannedChange,
                maxAcceleration: myCar.theoreticalMaxAcceleration
            );
        }

        // move the car closer to the middle of the lane (if no other cars are in the way)
        public static float ConvergeToMiddleOfLane(Car myCar)
        {
            var lane = myCar.lane >= 0
                ? myCar.lane
                : -myCar.lane + 0.5f;
            var decimalsOfLane = lane - (int) lane;
            
            if (decimalsOfLane <= myCar.laneChangingRate)
                return Mathf.Round(myCar.lane);

            var direction = decimalsOfLane > 0.5
                ? Direction.Right
                : Direction.Left;

            var plannedChange = myCar.laneChangingRate * (int) direction;
            var sideCars = CarsNextTo(myCar, direction, plannedChange).ToList();
            
            return sideCars.Count == 0
                ? myCar.lane + plannedChange
                : myCar.lane;
        }
        
        // the cars that would be in the way of myCar if it were to change lanes in `direction` by `plannedChange`
        public static IEnumerable<Car> CarsNextTo(Car myCar, Direction direction, float plannedChange)
        {
            bool NextToMyCar(Car otherCar) => myCar.AbsDistanceTo(otherCar) < myCar.length/2 + otherCar.length/2;

            bool DirectlyNextToMyCar(Car otherCar)
            {
                var sideDistance = myCar.lane - otherCar.lane;
                switch (direction)
                {
                    case Direction.Left:
                        return sideDistance >= -1 + plannedChange && sideDistance < 0;
                    case Direction.Right:
                        return sideDistance <= 1 + plannedChange && sideDistance > 0;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
            }

            return myCar.track.cars.LookBack(myCar).TakeWhile(NextToMyCar).Where(DirectlyNextToMyCar)
                .Concat(myCar.track.cars.LookAhead(myCar).TakeWhile(NextToMyCar).Where(DirectlyNextToMyCar));
        }
        
        public enum Direction
        {
            Left = -1, // turn left
            None = 0, // don't change lanes
            Right = +1 // turn right
        }
        
        // calculate the direction in which the car needs to move
        public static Direction LaneChangeDirection(HashSet<LaneType> currentLaneTypes, LaneType wantedLaneType)
        {
            if (currentLaneTypes.Contains(wantedLaneType))
                return Direction.None;
            
            switch (wantedLaneType)
            {
                case LaneType.LeftTurn:
                    return Direction.Left;

                case LaneType.Through:
                    return currentLaneTypes.Contains(LaneType.LeftTurn)
                        ? Direction.Right
                        : Direction.Left;

                case LaneType.RightTurn:
                    return Direction.Right;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(wantedLaneType), wantedLaneType, null);
            }
        }
    }
}