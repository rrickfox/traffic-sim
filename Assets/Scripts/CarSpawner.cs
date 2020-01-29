using UnityEngine;
using System.Collections.Generic;
using DataTypes;

class CarSpawner : ScriptableObject
{
    private GameObject _carPrefab;
    private GameObject _roadPrefab;

    public CarSpawner (GameObject carPrefab, GameObject roadPrefab)
    {
        _carPrefab = carPrefab;
        _roadPrefab = roadPrefab;
    }

    public void displayCar(Car car)
    {
        Vector2 position = car.road.GetPosition(car.positionOnRoad, car.lane);
        float angle = RoadAngle(car.road);

        Vector3 spawnPoint = new Vector3(position.x, _roadPrefab.transform.localScale.y / 2 + _carPrefab.transform.localScale.y / 2, position.y);
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        GameObject tempCar = Instantiate(_carPrefab, spawnPoint, rotation);
        tempCar.name = "Car_" + CarId.id;
        car.carTransform = tempCar.transform;
    }

    public float RoadAngle(Edge road)
    {
        return Vector2.SignedAngle(road.other.position - road.position, Vector2.right);
    }
}