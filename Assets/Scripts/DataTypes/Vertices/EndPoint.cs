using Utility;
using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public class EndPoint : Vertex
    {
        public override GameObject prefab { get; } = EMPTY_PREFAB;

        private Edge _edge { get; }
        public bool starting { get; private set; }
        public bool ending { get; private set; }
        // ticks before a car spawns on a lane (index)
        private Frequencies _frequencies { get; }
        // cumulative Probabilities of choosing a vertex to route to
        private Dictionary<Edge, int> _weights { get; }
        public Dictionary<Vertex, List<RouteSegment>> routingTable { get; } = new Dictionary<Vertex, List<RouteSegment>>();

        public EndPoint(Edge edge, Frequencies frequencies, Dictionary<Edge, int> weights) : base(edge)
        {
            _edge = edge;
            _frequencies = frequencies;
            _weights = weights;
            starting = _edge.outgoingLanes.Count > 0;
            ending = _edge.incomingLanes.Count > 0;

            if (edge.incomingLanes.Any(lane => lane.types.Count > 1 || !lane.types.Contains(LaneType.Through)))
                throw new NetworkConfigurationError("All lanes going into an EndPoint have to be of type Through");
            
            // subscribe to updates
            _publisher = new ObjectPublisher(typePublisher);
            _publisher.Subscribe(SpawnCars);
            _publisher.Subscribe(DespawnCars);
        }

        public void SpawnCars()
        {
            foreach (var lane in _frequencies.CurrentActiveIndices())
            {
                // only spawn car if no other car is in range of beginning
                var firstCarOnLane = _edge.cars.FirstOrDefault(c => c.lane == lane);
                if(firstCarOnLane == null || firstCarOnLane.positionOnRoad > firstCarOnLane.length * 3)
                {
                    // TODO: Temporarily only spawn car if route starts with current lane
                    // will be removed when cars actually switch lanes
                    List<RouteSegment> route;
                    do
                    {
                        route = routingTable[Utility.Random.Choose(_weights)].ToList();
                    } while (!_edge.outgoingLanes.ElementAt(lane).types.Contains(route.First().laneType));
                    new Car(lane, route);
                }
            }
        }

        public void DespawnCars()
        {
            // removes incoming cars
            foreach (var car in _edge.other.cars.ToList().Where(car => car.positionOnRoad >= _edge.length))
            {
                _edge.other.cars.Remove(car);
                car.Dispose();
            }
        }

        public override LaneType SubRoute(Edge comingFrom, Edge to) => LaneType.Through;
        public override bool IsRoutePossible(Edge comingFrom, Edge to) => true;
    }
}