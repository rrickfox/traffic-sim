using System.Collections.Immutable;
using UnityEngine;

namespace DataTypes
{
    class EndPoint : Vertex
    {
        private CarSpawner _spawner;
        private CarDespawner _despawner;

        public EndPoint(Edge edge, GameObject carPrefab, GameObject roadPrefab, int[] spawnFrequencies)
            : base(ImmutableArray.Create(edge))
        {
            _spawner = new CarSpawner(carPrefab, roadPrefab, edge, spawnFrequencies);
            _despawner = new CarDespawner(edge.other);
        }

        public void SpawnCars()
        {
            _spawner.SpawnCars();
        }

        public void DespawnCars()
        {
            _despawner.RemoveCars();
        }
    }
}