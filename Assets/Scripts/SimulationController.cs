using UnityEngine;
using System.Collections.Generic;
using DataTypes;
public class SimulationController : MonoBehaviour
{
    public GameObject roadPrefab;
    public GameObject carPrefab;

     private RoadSpawner _roadSpawner;
     private List<Road> _roads = new List<Road>();
     private int _idRoad = 0;
     private List<List<Car>> _cars = new List<List<Car>>();
     private int _idCar = 0;
     
     public void Start()
     {
        _roadSpawner = new RoadSpawner(roadPrefab);

         Vector2 startPos = new Vector2(-140, 0);
         Vector2 endPos = new Vector2(140, 0);
         createRoad(startPos, endPos, 1, 0);
     }

     public void createRoad(Vector2 startPos, Vector2 endPos, int lanesStartToEnd, int lanesEndToStart)
     {
         Road tempRoad = new Road(_idRoad, startPos, endPos, lanesStartToEnd, lanesEndToStart);
         _idRoad++;
         _roads.Add(tempRoad);
         _roadSpawner.displayRoad(tempRoad);
     }
}