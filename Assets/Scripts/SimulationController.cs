using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using MoreLinq;

// ruft Autos zur Bewegung auf
// -> hat einzige Update-Funktion ("interne Zeit")
// erstellt Straße (vorläufig)
public class SimulationController : MonoBehaviour
{
    public GameObject roadPrefab;
    public GameObject carPrefab;

    // Point1, Point2
    public Vector2 startPoint = new Vector2(-140, -50);
    public Vector2 controlPoint = new Vector2(0, 50);
    public Vector2 endPoint = new Vector2(140, -50);

    // spawn frequency
    public int lane1To2 = 100;
    public int lane2To1 = 100;

    private RoadSpawner _roadSpawner;
    private List<Edge> _roads = new List<Edge>();
    private List<EndPoint> _spawnPoints = new List<EndPoint>();
    
    public void Start()
    {
        _roadSpawner = new RoadSpawner(roadPrefab);
        
        var lanes1To2 = new List<Lane> { new Lane(new HashSet<LaneType> {LaneType.Through}) };
        var lanes2To1 = new List<Lane> { new Lane(new HashSet<LaneType> {LaneType.Through}) };
        
        // spawn frequency
        var frequencyLanes1To2 = new[] { lane1To2 };
        var frequencyLanes2To1 = new[] { lane2To1 };

        // road Shape
        var curve = new BezierCurve(startPoint, controlPoint, endPoint);
        var shape = new RoadShape(new List<BezierCurve>(){curve});

        // Road create..
        var road = _roadSpawner.CreateRoad(shape, lanes1To2, lanes2To1);

        // EndPoint creation
        var pointA = new EndPoint(road, carPrefab, roadPrefab, frequencyLanes1To2);
        _spawnPoints.Add(pointA);
        var pointB = new EndPoint(road.other, carPrefab, roadPrefab, frequencyLanes2To1);
        _spawnPoints.Add(pointB);
        _roads.Add(road);
        Vertex.StartPathfinding(_spawnPoints.Cast<Vertex>().ToHashSet());
    }

    void FixedUpdate()
    {
        CheckForSpawn();
        MoveCars();
        CheckForDespawn();
    }

    public void CheckForSpawn()
    {
        foreach(var vertex in _spawnPoints)
        {
            vertex.SpawnCars();
        }
    }

    public void CheckForDespawn()
    {
        foreach(var vertex in _spawnPoints)
        {
            vertex.DespawnCars();
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
            // move all cars driving on the opposite lanes
            foreach(var car in road.other.cars)
            {
                car.Move();
            }
        }
    }
}
