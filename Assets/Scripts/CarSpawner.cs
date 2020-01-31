using UnityEngine;
using DataTypes;

class CarSpawner
{
    private GameObject _carPrefab;
    private GameObject _roadPrefab;
    // the edge on which to spawn new cars
    private Edge _edge;
    // ticks before a car spawns on a lane (index)
    private int[] _spawnFrequencies;
    // counter for ticks since start
    private int _ticks = 0;

    public CarSpawner(GameObject carPrefab, GameObject roadPrefab, Edge edge, int[] spawnFrequencies)
    {
        _carPrefab = carPrefab;
        _roadPrefab = roadPrefab;
        _edge = edge;
        _spawnFrequencies = spawnFrequencies;
    }

    public void SpawnCars()
    {
        for(var lane = 0; lane < _edge.outgoingLanes.Count; lane++)
        {
            if(_ticks % _spawnFrequencies[lane] == 0)
            {
                CreateCar(_edge, 0, lane);
            }
        }
        _ticks++;
    }
    
    public void CreateCar(Edge road, float positionOnRoad, float lane)
    {
        // construct car
        var car = new Car(road, 0, lane);
        
        // display the car graphically
        var position = road.GetAbsolutePosition(positionOnRoad, lane);
        var angle = RoadAngle(car.road);
        var spawnPoint = new Vector3(position.x, _roadPrefab.transform.localScale.y / 2 + _carPrefab.transform.localScale.y / 2, position.y);
        var rotation = Quaternion.Euler(0, angle, 0);
        var carGameObject = Object.Instantiate(_carPrefab, spawnPoint, rotation);
        carGameObject.name = "Car_" + CarId.id;
        car.carTransform = carGameObject.transform;
    }

    public float RoadAngle(Edge road)
    {
        return Vector2.SignedAngle(road.other.position - road.position, Vector2.right);
    }
}