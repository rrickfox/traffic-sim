using Utility;
using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;

namespace DataTypes
{
    public class EndPoint : Vertex
    {
        private Edge _edge { get; }
        private GameObject _carPrefab { get; }
        // ticks before a car spawns on a lane (index)
        private Frequencies _frequencies { get; }
        // cumulative Probabilities of choosing a vertex to route to
        private List<double> _cumulativeProbabilities { get; }
        public Dictionary<IVertex, List<RouteSegment>> routingTable { get; } = new Dictionary<IVertex, List<RouteSegment>>();
        
        // updates only happen after all car updates
        public static TypePublisher typePublisher { get; } = TypePublisher.Create<EndPoint>(Car.typePublisher);

        public EndPoint(Edge edge, GameObject carPrefab, Frequencies frequencies, int[] weights) : base(edge)
        {
            _edge = edge;
            _carPrefab = carPrefab;
            _frequencies = frequencies;
            _cumulativeProbabilities = MathUtils.CalculateCumulative(weights);

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
                new Car(_carPrefab, lane, routingTable[Utility.Random.Choose(
                    cumulativeProbabilities: _cumulativeProbabilities, 
                    destinations: routingTable.Where(kvp => kvp.Value != null).Select(kvp => kvp.Key).ToList())].ToList());
            }
        }

        public void DespawnCars()
        {
            // removes incoming cars
            foreach (var car in _edge.other.cars.ToList().Where(car => car.positionOnRoad >= _edge.length))
            {
                car.Dispose();
                _edge.other.cars.Remove(car);
            }
        }

        public override LaneType SubRoute(Edge comingFrom, Edge to) => LaneType.Through;
    }
}