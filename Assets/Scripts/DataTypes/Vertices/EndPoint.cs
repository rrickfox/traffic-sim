using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataTypes
{
    public class EndPoint : InvisibleVertex<EndPoint, EndPointBehaviour>
    {
        private Edge _edge { get; }
        private GameObject _carPrefab { get; }
        // ticks before a car spawns on a lane (index)
        private Frequencies _frequencies { get; }
        public Dictionary<IVertex, List<Edge>> routingTable { get; } = new Dictionary<IVertex, List<Edge>>();

        public EndPoint(Edge edge, GameObject carPrefab, Frequencies frequencies) : base(edge)
        {
            _edge = edge;
            _carPrefab = carPrefab;
            _frequencies = frequencies;
        }

        public void SpawnCars()
        {
            foreach (var lane in _frequencies.CurrentActiveIndices())
            {
                new Car(_carPrefab, _edge, 0, lane);
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