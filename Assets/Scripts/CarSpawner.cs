using UnityEngine;
using System.Collections.Generic;
using DataTypes;

class CarSpawner : ScriptableObject
{
    private CONSTANTS _constants = new CONSTANTS();
    private List<GameObject> _cars = new List<GameObject>();
    private List<GameObject> _roads = new List<GameObject>();
    private GameObject _carPrefab;
    private GameObject _roadPrefab;

    public CarSpawner (GameObject carPrefab, GameObject roadPrefab, List<GameObject> roads)
    {
        _carPrefab = carPrefab;
        _roadPrefab = roadPrefab;
        _roads = roads;
    }

    public void displayCar(Car car)
    {
        Vector2 position = Vector2.Lerp(car.road.anchors1.position, car.road.anchors2.position, car.positionOnRoad/car.road.length);
        Vector3 spawnPoint = new Vector3(position.x, _roadPrefab.transform.localScale.y / 2 + _carPrefab.transform.localScale.y / 2, position.y);
        float angle = 0;
        float offset = (car.road.lanes1To2 - 1) * _constants.LANE_WIDTH * car.lane / 2f; // TODO: Check if different for other direction
        if(car.direction == Direction.direction1To2)
        {
            angle = _roads[car.road.id].transform.rotation.y;
        } else
        {
            angle = (_roads[car.road.id].transform.rotation.y + 180) % 360;
        }
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        GameObject tempCar = Instantiate(_carPrefab, spawnPoint, rotation);
        tempCar.transform.position += tempCar.transform.right * offset;
        tempCar.name = "Car_" + car.id;
        _cars.Add(tempCar);
    }
}