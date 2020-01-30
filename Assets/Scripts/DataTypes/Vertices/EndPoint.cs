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

        public void spawnCars()
        {
            for(var i = 0; i < edge.outgoingLanes.Count; i++)
            {
                if(_ticks % _spawnFrequency[i] == 0)
                {
                    createCar(i);
                }
            }
            _ticks++;
        }

        public void despawnCars()
        {
            _despawner.removeCars();
        }

        public void createCar(float lane)
        {
            var tempCar = new Car(edge, 0, lane);
            edge.cars.Add(tempCar);
            _spawner.displayCar(tempCar);
        }
    }
}