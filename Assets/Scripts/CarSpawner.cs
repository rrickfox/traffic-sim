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
        Vector2 position = Vector2.zero;
        float angle = 0;
        float offset = 0;
        if(car.direction == Direction.direction1To2)
        {
            position = car.road.node1.position;
            angle = _roads[car.road.id].transform.rotation.y;
            offset = (((car.road.lanes1To2 + car.road.lanes2To1) / 2) - car.road.lanes1To2 + 0.5f + car.lane) * CONSTANTS.LANE_WIDTH ;
        } else
        {
            position = car.road.node2.position;
            angle = (_roads[car.road.id].transform.rotation.y + 180) % 360;
            offset = (((car.road.lanes1To2 + car.road.lanes2To1) / 2) - car.road.lanes2To1 + 0.5f + car.lane) * CONSTANTS.LANE_WIDTH ;
        }

        Vector3 spawnPoint = new Vector3(position.x, _roadPrefab.transform.localScale.y / 2 + _carPrefab.transform.localScale.y / 2, position.y);
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        GameObject tempCar = Instantiate(_carPrefab, spawnPoint, rotation);
        tempCar.transform.position += tempCar.transform.right * offset;
        tempCar.name = "Car_" + car.id;
        _cars.Add(tempCar);
    }
}