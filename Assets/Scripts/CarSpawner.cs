using UnityEngine;
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

    public void DisplayCar(Car car)
    {
        var position = car.road.GetAbsolutePosition(car.positionOnRoad, car.lane);
        var angle = RoadAngle(car.road);

        var spawnPoint = new Vector3(position.x, _roadPrefab.transform.localScale.y / 2 + _carPrefab.transform.localScale.y / 2, position.y);
        var rotation = Quaternion.Euler(0, angle, 0);
        var tempCar = Instantiate(_carPrefab, spawnPoint, rotation);
        tempCar.name = "Car_" + CarId.id;
        car.carTransform = tempCar.transform;
    }

    public float RoadAngle(Edge road)
    {
        return Vector2.SignedAngle(road.other.position - road.position, Vector2.right);
    }
}