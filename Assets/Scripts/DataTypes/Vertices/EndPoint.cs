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

        public EndPoint(Edge edge, GameObject carPrefab, Frequencies frequencies, int[] weights) : base(edge)
        {
            _edge = edge;
            _carPrefab = carPrefab;
            _frequencies = frequencies;
            _weights = weights;

            // reset LaneTypes of lanes, needed for SubRouting
            edge.other.ResetOutgoingLaneTypes();
        }

        public void SetWeights()
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
            foreach (var car in _edge.cars.ToList().Where(car => car.positionOnRoad >= _edge.length))
            {
                car.Dispose();
                _edge.cars.Remove(car);
            }
        }

        public override LaneType SubRoute(Edge from, Edge to)
        {
            return LaneType.Through;
        }
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