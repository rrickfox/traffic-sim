using System.Collections.Immutable;
using UnityEngine;

namespace DataTypes
{
    class EndPoint : Vertex
    {
        protected Edge edge;
        private CarSpawner _spawner;
        private CarDespawner _despawner;
        // ticks before a car spawns on a lane (index)
        private float[] _spawnFrequency;
        // counter for ticks since start
        private int _ticks = 0;

        public EndPoint(Edge edge, GameObject carPrefab, GameObject roadPrefab, float[] spawnFrequency)
            : base(ImmutableArray.Create(edge))
        {
            this.edge = edge;
            _spawner = new CarSpawner(carPrefab, roadPrefab);
            _despawner = new CarDespawner(edge.other);
            _spawnFrequency = spawnFrequency;
        }

        public void SpawnCars()
        {
            for(var i = 0; i < edge.outgoingLanes.Count; i++)
            {
                if(_ticks % _spawnFrequency[i] == 0)
                {
                    CreateCar(i);
                }
            }
            _ticks++;
        }

        public void DespawnCars()
        {
            _despawner.RemoveCars();
        }

        public void CreateCar(float lane)
        {
            var tempCar = new Car(edge, 0, lane);
            edge.cars.Add(tempCar);
            _spawner.DisplayCar(tempCar);
        }
    }
}