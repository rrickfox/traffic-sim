using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using static Pathfinding.Pathfinding;

// erstellt alle Objekte manuell (vorläufig)
public class SimulationController : MonoBehaviour
{
    public GameObject roadPrefab;
    public GameObject carPrefab;

    // Point1, Point2
    public Vector2 startPoint = new Vector2(-140, -50);
    public Vector2 controlPoint = new Vector2(0, 50);
    public Vector2 endPoint = new Vector2(140, -50);

    // spawn frequency
    public int lane1To2 = 5;
    public int lane2To1 = 5;

    private List<Edge> _roads = new List<Edge>();
    private List<EndPoint> _spawnPoints = new List<EndPoint>();
    
    public void Start()
    {
        var lanes1To2 = new List<Lane> { new Lane(new HashSet<LaneType> {LaneType.Through}) };
        var lanes2To1 = new List<Lane> { new Lane(new HashSet<LaneType> {LaneType.Through}) };
        
        // spawn frequency
        var frequencyLanes1To2 = new Frequencies(new[] {lane1To2});
        var frequencyLanes2To1 = new Frequencies(new[] {lane2To1});

        // road Shape
        var curve = new BezierCurve(startPoint, controlPoint, endPoint);
        var shape = new RoadShape(new List<BezierCurve>(){curve});

        // Road create..
        var road = new Edge(roadPrefab, shape, lanes1To2, lanes2To1);

        // EndPoint creation
        var pointA = new EndPoint(road, carPrefab, frequencyLanes1To2);
        _spawnPoints.Add(pointA);
        var pointB = new EndPoint(road.other, carPrefab, frequencyLanes2To1);
        _spawnPoints.Add(pointB);
        _roads.Add(road);

        #region 2.road
        var lanes1To2_1 = new List<Lane> { new Lane(new HashSet<LaneType> {LaneType.Through}), new Lane(new HashSet<LaneType>{LaneType.Through}) };
        var lanes2To1_1 = new List<Lane> { new Lane(new HashSet<LaneType> {LaneType.Through}) };

        var frequencyLanes1To2_1 = new Frequencies(new[] { 100, 66 });
        var frequencyLanes2To1_1 = new Frequencies(new[] { 100 });

        var curve_1 = new BezierCurve(new Vector2(-50, 50), new Vector2(0, 50), new Vector2(50, 50));
        var shape_1 = new RoadShape(new List<BezierCurve>() { curve_1 });

        var road_1 = new Edge(roadPrefab, shape_1, lanes1To2_1, lanes2To1_1);

        var pointC = new EndPoint(road_1, carPrefab, frequencyLanes1To2_1);
        _spawnPoints.Add(pointC);
        var pointD = new EndPoint(road_1.other, carPrefab, frequencyLanes2To1_1);
        _spawnPoints.Add(pointD);
        _roads.Add(road_1);
        #endregion

        StartPathfinding(_spawnPoints.Cast<IVertex>().ToList());
    }
}
