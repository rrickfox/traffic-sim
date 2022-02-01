using System.Collections.Generic;
using System.Linq;
using DataTypes.Drivers;
using Events;
using UnitsNet;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;
using static Utility.Formulas;
using Random = UnityEngine.Random;

namespace DataTypes
{
    public class Car : GameObjectData, ISortableListNode
    {
        public override GameObject prefab { get; } = CAR_PREFABS.Keys.ElementAt(Utility.Random.RANDOM.Next(CAR_PREFABS.Keys.Count));
        // have multiple publishers for each action
        // the order of execution for each update is: change lanes -> accelerate -> execute the movements
        public static TypePublisher LANE_CHANGE_PUBLISHER { get; } = new TypePublisher();
        public static TypePublisher ACCELERATE_PUBLISHER { get; } = new TypePublisher(LANE_CHANGE_PUBLISHER);
        public static TypePublisher MOVE_PUBLISHER { get; } = new TypePublisher(ACCELERATE_PUBLISHER);
        public static TypePublisher typePublisher { get; } = new TypePublisher(TrafficLight.typePublisher, MOVE_PUBLISHER);

        public ISortableListNode previous { get; set; }
        public ISortableListNode next { get; set; }

        public ITrack track;
        public List<RouteSegment> route { get; }
        public RouteSegment segment { get; private set; }

        // https://de.wikipedia.org/wiki/Liste_von_Gr%C3%B6%C3%9Fenordnungen_der_Beschleunigung
        // the maximum acceleration this car could theoretically have
        public Acceleration theoreticalMaxAcceleration { get; } = Acceleration.FromMetersPerSecondSquared(3);
        // the maximum acceleration allowed to still be able to change lanes (specified by LaneChangingDriver)
        public Acceleration maxAcceleration { get; private set; } = Acceleration.FromMetersPerSecondSquared(3);
        public Acceleration brakingDeceleration { get; } = Acceleration.FromMetersPerSecondSquared(-4);
        public Acceleration maxBrakingDeceleration { get; } = Acceleration.FromMetersPerSecondSquared(-8);
        public Length bufferDistance => Length.FromMeters(Mathf.Max((float) length.Meters / 4f, Mathf.Round((float) speed.KilometersPerHour / 4f * 10f) / 10f));
        public Length length { get; }
        // any braking distance below this is theoretically impossible
        public Length finalDistance => BrakingDistance(speed, -maxBrakingDeceleration);
        // minimum value of criticalDistance
        public Length criticalBufferDistance => bufferDistance * 2f + SECTION_BUFFER_LENGTH.DistanceUnitsToLength();
        // cars should start slowing down at this distance
        public Length criticalDistance => Max(BrakingDistance(speed, 0.5 * -brakingDeceleration), criticalBufferDistance);
        // the speed with which to change lanes
        public float laneChangingRate { get; } = 0.02f;

        public Length positionOnRoad { get; private set; } = Length.Zero;
        public float lane { get; private set; }
        public HashSet<LaneType> laneTypes => segment.edge.outgoingLanes[Mathf.RoundToInt(lane)].types;
        public Speed speed { get; private set; }
        public Acceleration acceleration { get; private set; }

        public Car(int lane, List<RouteSegment> route)
        {
            length = Length.FromMeters(CAR_PREFABS[prefab]);

            this.route = route;
            segment = route.PopAt(0);
            track = segment.edge;
            track.cars.AddFirst(this);
            this.lane = lane;
            // set starting speed to half of the speed limit
            speed = 0.5 * track.speedLimit;

            // give car a random color
            gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color = Random.ColorHSV();

            UpdatePosition();

            // subscribe to updates (see above for the execution order)
            var laneChangePublisher = new ObjectPublisher(LANE_CHANGE_PUBLISHER);
            laneChangePublisher.Subscribe(ChangeLane);
            var acceleratePublisher = new ObjectPublisher(ACCELERATE_PUBLISHER);
            acceleratePublisher.Subscribe(SelectAccelerator);
            var movePublisher = new ObjectPublisher(MOVE_PUBLISHER);
            movePublisher.Subscribe(ExecuteMove);
            _allPublishers.Add(laneChangePublisher);
            _allPublishers.Add(acceleratePublisher);
            _allPublishers.Add(movePublisher);
        }

        private void ChangeLane()
        {
            switch (track)
            {
                // only change lanes if the car is on a road
                case Edge _:
                    var direction = LaneChangingDriver.LaneChangeDirection(laneTypes, segment.laneType);
                    
                    if (direction == LaneChangingDriver.Direction.None)
                        // don't change lanes but try to stay in the middle of the road
                        lane = LaneChangingDriver.ConvergeToMiddleOfLane(this);
                    else
                        // change lanes and calculate an acceleration limit 
                        (lane, maxAcceleration) = LaneChangingDriver.ChangeLane(this, direction);
                    
                    break;
            }
        }
        
        // accelerate depending on the context
        private void SelectAccelerator()
        {
            var frontCar = GetFrontCar();
            
            if (frontCar == null && track.light != null)
                acceleration = TrafficLightDriver.LightAcceleration(this);
            else
                acceleration = NormalDriver.NormalAcceleration(this, frontCar);
        }

        // Returns the Car in front of the current Car
        public Car GetFrontCar()
            => track.cars.LookAhead(this).FirstOrDefault(other => IsOnSameLane(other) && other.positionOnRoad > positionOnRoad);

        public bool IsOnSameLane(Car otherCar) => Mathf.Abs(lane - otherCar.lane) < 0.99;
        
        public Length AbsDistanceTo(Car otherCar) => Length.FromMeters(Mathf.Abs((float) (positionOnRoad - otherCar.positionOnRoad).Meters));
        
        private void ExecuteMove()
        {
            var newSpeed = speed.Plus(acceleration.Times(Formulas.TimeUnitsToTimeSpan(1)));
            if (newSpeed > track.speedLimit)
            {
                // enforce the speed limit
                speed = track.speedLimit;
                acceleration = Acceleration.Zero;
            }
            else if (newSpeed.MetersPerSecond <= 0)
            {
                // ensure that the car does not drive backwards
                speed = Speed.Zero;
                acceleration = Acceleration.Zero;
            }
            else
            {
                speed = newSpeed;
            }

            positionOnRoad += speed.Times(Formulas.TimeUnitsToTimeSpan(1));

            UpdatePosition();

            // if car is at end of RouteSegment, get next routeSegment if there is one
            if (positionOnRoad >= track.length && route.Count > 0)
            {
                positionOnRoad -= track.length; // add overshot distance to new RouteSegment
                lane = Mathf.Round(lane);
                switch (track)
                {
                    case SectionTrack _:
                        track.cars.Remove(this);
                        segment = route.PopAt(0);
                        lane = track.newLane;
                        track = segment.edge;
                        track.cars.AddFirst(this);
                        break;

                    case Edge _:
                        track.cars.Remove(this);
                        var laneToTrack = segment.edge.other.vertex.routes[segment];
                        try
                        {
                            track = laneToTrack[(int) lane];
                        }
                        catch (KeyNotFoundException)
                        {
                            Debug.LogWarning("A Car could not find a way to turn to its desired road");
                            track = laneToTrack[laneToTrack.Keys.First()];
                        }
                        track.cars.AddFirst(this);
                        break;
                }
            }
        }

        private void UpdatePosition()
        {
            var roadPoint = track.GetAbsolutePosition(positionOnRoad, lane);
            transform.position = roadPoint.position.toWorld(transform.localScale.y / 2 + ROAD_HEIGHT);
            transform.rotation = Quaternion.Euler(0, Vector2.SignedAngle(roadPoint.forward, Vector2.right), 0);
        }
    }
}