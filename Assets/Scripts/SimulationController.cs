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

         Vector2 pos1 = new Vector2(-140, 0);
         Vector2 pos2 = new Vector2(140, 0);
         createRoad(pos1, pos2, 1, 0);
     }

     public void createRoad(Vector2 pos1, Vector2 pos2, int lanes1To2, int lanes2To1)
     {
         Node node1 = new Node(pos1, lanes1To2, lanes2To1);
         Node node2 = new Node(pos2, lanes2To1, lanes1To2);
         Road tempRoad = new Road(_idRoad, node1, node2, lanes1To2, lanes2To1);
         _idRoad++;
         _roads.Add(tempRoad);
         _roadSpawner.displayRoad(tempRoad);
     }
}