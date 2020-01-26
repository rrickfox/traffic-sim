using System.Collections.Immutable;
using UnityEngine;

namespace DataTypes
{
    class EndPoint : Vertex
    {
        protected Edge edge;
        private CarSpawner _spawner;

        public EndPoint(Edge edge, GameObject carPrefab, GameObject roadPrefab)
            : base(ImmutableArray.Create(edge))
        {
            this.edge = edge;
            _spawner = new CarSpawner(carPrefab, roadPrefab);
        }

        public void spawnCars(Edge road)
        {
                        
        }

        public void createCar(float lane)
        {
            Car tempCar = new Car(edge, 0, lane);
            edge.cars.Add(tempCar);
            _spawner.displayCar(tempCar);
        }
    }
}