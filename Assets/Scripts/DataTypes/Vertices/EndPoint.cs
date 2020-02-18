using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataTypes
{
    public class EndPoint : Vertex<EndPoint, EndPointBehaviour>
    {
        private Edge _edge { get; }
        private GameObject _carPrefab { get; }
        // ticks before a car spawns on a lane (index)
        private Frequencies _frequencies { get; }
        // weighed choosing of vertices to route to
        private RouteProbabilities _routeProbabilities;
        private int[] _weights { get; }
        public Dictionary<IVertex, List<RouteSegment>> routingTable { get; } = new Dictionary<IVertex, List<RouteSegment>>();
        private static readonly HashSet<LaneType> _ONLY_THROUGH = new HashSet<LaneType> {LaneType.Through};

        public EndPoint(Edge edge, GameObject carPrefab, Frequencies frequencies, int[] weights) : base(edge)
        {
            _edge = edge;
            _carPrefab = carPrefab;
            _frequencies = frequencies;
            _weights = weights;

            if (edge.incomingLanes.Any(lane => lane.types.Equals(_ONLY_THROUGH)))
                throw new Exception("All lanes going into an EndPoint have to be of type Through");
        }

        public void CalculateRouteProbabilities()
        {
            _routeProbabilities = new RouteProbabilities(_weights, routingTable.Where(kvp => kvp.Value != null).Select(kvp => kvp.Key).ToList());
        }

        public void SpawnCars()
        {
            foreach (var lane in _frequencies.CurrentActiveIndices())
            {
                new Car(_carPrefab, lane, routingTable[_routeProbabilities.Choose()].ToList());
            }
        }

        public void DespawnCars()
        {
            foreach (var car in _edge.other.cars.ToList().Where(car => car.positionOnRoad >= _edge.length))
            {
                car.Dispose();
                _edge.other.cars.Remove(car);
            }
        }

        public override LaneType SubRoute(Edge from, Edge to) => LaneType.Through;
    }

    public class EndPointBehaviour : VertexBehaviour<EndPoint>
    {
        private void FixedUpdate()
        {
            _data.SpawnCars();
            _data.DespawnCars();
        }
    }
}