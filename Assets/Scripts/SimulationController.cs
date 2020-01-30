using UnityEngine;
using System.Collections.Generic;
using DataTypes;

// ruft Autos zur Bewegung auf
// -> hat einzige Update-Funktion ("interne Zeit")
// erstellt Straße (vorläufig)
public class SimulationController : MonoBehaviour
{
    public GameObject roadPrefab;
    public GameObject carPrefab;

    // Point1, Point2
    public Vector2 pos1 = new Vector2(-140, 0);
    public Vector2 pos2 = new Vector2(140, 0);

    // spawn frequency
    public float lane1To2 = 100;
    public float lane2To1 = 100;

    private RoadSpawner _roadSpawner;
    private List<Edge> _roads = new List<Edge>();
    private List<EndPoint> _spawnPoints = new List<EndPoint>();
     
    public void Start()
    {
        _roadSpawner = new RoadSpawner(roadPrefab);

        // Definition lanes1To2
        var lane1To2_0_types = new HashSet<LaneType>(); 
        lane1To2_0_types.Add(LaneType.Through);
        var lane1To2_0 = new Lane(lane1To2_0_types);

        var lanes1To2 = new List<Lane>()
        {
            lane1To2_0
        };

        // Definition lanes2To1
        var lane2To1_0_types = new HashSet<LaneType>();
        lane1To2_0_types.Add(LaneType.Through);
        var lane2To1_0 = new Lane(lane2To1_0_types);

        var lanes2To1 = new List<Lane>()
        {
            lane2To1_0
        };

        // Road create..
        var view = CreateRoad(pos1, pos2, lanes1To2, lanes2To1);

        // spawn frequency
        var freqLane1To2 = new float[1];
        freqLane1To2[0] = lane1To2;
        var freqLane2To1 = new float[1];
        freqLane2To1[0] = lane2To1;

        // EndPoint creation
        var road = new Edge(view, null, view.other, null);
        var pointA = new EndPoint(road, carPrefab, roadPrefab, freqLane1To2);
        _spawnPoints.Add(pointA);
        var pointB = new EndPoint(road.other, carPrefab, roadPrefab, freqLane2To1);
        _spawnPoints.Add(pointB);
        road.vertex = pointA;
        road.other.vertex = pointB;
        _roads.Add(road);
    }

    void FixedUpdate()
    {
        CheckForSpawn();
        MoveCars();
        CheckForDespawn();
    }

    public RoadView CreateRoad(Vector2 pos1, Vector2 pos2, List<Lane> lanes1To2, List<Lane> lanes2To1)
    {
        var view = new RoadView(new RoadShape(), pos1, pos2, lanes1To2, lanes2To1);
        _roadSpawner.DisplayRoad(view);
        return view;
    }

    public void CheckForSpawn()
    {
        foreach(var vertex in _spawnPoints)
        {
            vertex.spawnCars();
        }
    }

    public void CheckForDespawn()
    {
        foreach(var vertex in _spawnPoints)
        {
            vertex.despawnCars();
        }
    }

    public void MoveCars()
    {
        foreach(var road in _roads)
        {
            foreach(var car in road.cars)
            {
                car.Move();
            }
            foreach(var car in road.other.cars)
            {
                car.Move();
            }
        }
    }
}
