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
        private int[] _spawnFrequencies;
        // counter for ticks since start
        private int _ticks = 0;

        public EndPoint(Edge edge, GameObject carPrefab, GameObject roadPrefab, int[] spawnFrequencies)
            : base(ImmutableArray.Create(edge))
        {
            _edge = edge;
            _spawner = new CarSpawner(carPrefab, roadPrefab);
            _despawner = new CarDespawner(edge.other);
            _spawnFrequencies = spawnFrequencies;
        }

        public void SpawnCars()
        {
            for(var lane = 0; lane < _edge.outgoingLanes.Count; lane++)
            {
                if(_ticks % _spawnFrequencies[lane] == 0)
                {
                    _spawner.CreateCar(_edge, 0, lane);
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