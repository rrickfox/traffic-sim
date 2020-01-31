using System.Collections.Immutable;
using UnityEngine;

namespace DataTypes
{
    class EndPoint : Vertex
    {
        private Edge _edge;
        private CarSpawner _spawner;
        private CarDespawner _despawner;
        // ticks before a car spawns on a lane (index)
        private float[] _spawnFrequencies;
        // counter for ticks since start
        private int _ticks = 0;

        public EndPoint(Edge edge, GameObject carPrefab, GameObject roadPrefab, float[] spawnFrequencies)
            : base(ImmutableArray.Create(edge))
        {
            _edge = edge;
            _spawner = new CarSpawner(carPrefab, roadPrefab);
            _despawner = new CarDespawner(edge.other);
            _spawnFrequencies = spawnFrequencies;
        }

        public void SpawnCars()
        {
            for(var i = 0; i < _edge.outgoingLanes.Count; i++)
            {
                if(_ticks % _spawnFrequencies[i] == 0)
                {
                    _spawner.CreateCar(_edge, 0, i);
                }
            }
            _ticks++;
        }

        public void DespawnCars()
        {
            _despawner.RemoveCars();
        }
    }
}