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
        private Dictionary<Edge, int> _weights { get; }
        public Dictionary<Vertex, List<RouteSegment>> routingTable { get; } = new Dictionary<Vertex, List<RouteSegment>>();
        
        // updates only happen after all car updates
        public static TypePublisher typePublisher { get; } = new TypePublisher(Car.typePublisher);

        public EndPoint(Edge edge, GameObject carPrefab, Frequencies frequencies, Dictionary<Edge, int> weights) : base(edge)
        {
            _edge = edge;
            _carPrefab = carPrefab;
            _frequencies = frequencies;
            _weights = weights;

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
                var endPoint = Utility.Random.Choose(_weights);
                //TODO
                //Debug.Log(_edge.originPoint.position);
                //Debug.Log(endPoint.edges.First().originPoint.position);
                //Debug.Log(string.Join(", ", routingTable[endPoint]));
                //TODO
                Debug.Log(endPoint.edges.First().originPoint.position);
                var debug = "";
                debug += _edge.originPoint.position;
                debug += ": ";
                foreach(var route in routingTable)
                {
                    debug += "{";
                    debug += route.Key.edges.First().originPoint.position;
                    debug += ": ";
                    debug += "[";
                    foreach(var segment in route.Value)
                    {
                        debug += segment.track;
                        debug += ", ";
                    }
                    debug += "]}";
                }
                Debug.Log(debug);

                new Car(_carPrefab, lane, routingTable[endPoint].ToList());
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