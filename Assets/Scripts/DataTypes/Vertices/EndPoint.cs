using System.Collections.Immutable;
using UnityEngine;

namespace DataTypes
{
    class EndPoint : Vertex
    {
        private Edge _edge;
        private GameObject _carPrefab;
        private GameObject _roadPrefab;
        // ticks before a car spawns on a lane (index)
        private int[] _spawnFrequencies;
        // counter for ticks since start
        private int _ticks = 0;

        public EndPoint(Edge edge, GameObject carPrefab, GameObject roadPrefab, int[] spawnFrequencies)
            : base(ImmutableArray.Create(edge))
        {
            _edge = edge;
            _carPrefab = carPrefab;
            _roadPrefab = roadPrefab;
            _spawnFrequencies = spawnFrequencies;
        }

        public void SpawnCars()
        {
            for(var lane = 0; lane < _edge.outgoingLanes.Count; lane++)
            {
                if(_ticks % _spawnFrequencies[lane] == 0)
                {
                    CreateCar(lane);
                }
            }
            _ticks++;
        }
    
        private void CreateCar(float lane)
        {
            // construct car
            var car = new Car(_edge, 0, lane);
        
            // display the car graphically
            var position = _edge.GetAbsolutePosition(car.positionOnRoad, lane);
            var spawnPoint = new Vector3(position.x, _roadPrefab.transform.localScale.y / 2 + _carPrefab.transform.localScale.y / 2, position.y);
            var rotation = Quaternion.Euler(0, _edge.angle, 0);
            var carGameObject = Object.Instantiate(_carPrefab, spawnPoint, rotation);
            carGameObject.name = "Car_" + CarId.id;
            car.carTransform = carGameObject.transform;
        }

        public void DespawnCars()
        {
            foreach(var car in _edge.cars) 
            {
                if (car.positionOnRoad > _edge.length)
                {
                    Object.Destroy(car.carTransform.gameObject);
                    _edge.cars.Remove(car);
                }
            }
        }
    }
}