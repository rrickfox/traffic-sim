using UnityEngine;
using DataTypes;

class CarDespawner : ScriptableObject 
{
    public Edge road;
    public CarDespawner (Edge road)
    {
        this.road = road;
    }

    public void RemoveCars()
    {
        foreach(var car in road.cars) 
        {
            if (car.positionOnRoad > road.length)
            {
                Object.Destroy(car.carTransform.gameObject);
                road.cars.Remove(car);
            }
        }
    }
}
