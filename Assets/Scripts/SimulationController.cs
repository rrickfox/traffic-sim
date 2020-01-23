using UnityEngine;
using System.Collections.Generic;
using DataTypes;
using System.Collections.Immutable;
using System.Linq;
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

        // Point1, Point2
        Point point1 = new Point(new Vector2(-140, 0));
        Point point2 = new Point(new Vector2(140, 0));

        // Definition lanes1To2
        HashSet<LaneType> lane1To2_0_types = new HashSet<LaneType>(); 
        lane1To2_0_types.Add(LaneType.Through);
        Lane lane1To2_0 = new Lane(lane1To2_0_types);

        IEnumerable<Lane> lanes1To2 = new List<Lane>()
        {
            lane1To2_0
        }.AsEnumerable();

        //lane2To1
        HashSet<LaneType> lane2To1_0_types = new HashSet<LaneType>();
        lane1To2_0_types.Add(LaneType.Through);
        Lane lane2To1_0 = new Lane(lane2To1_0_types);

        IEnumerable<Lane> lanes2To1 = new List<Lane>()
        {
            lane2To1_0
        }.AsEnumerable();

        // Road create..
        createRoad(point1, point2, lanes1To2, lanes2To1);
     }

     public void createRoad(Point point1, Point point2, IEnumerable<Lane> lanes1To2, IEnumerable<Lane> lanes2To1)
     {
        Road tempRoad = new Road(_idRoad, new RoadShape(), point1, point2, lanes2To1, lanes1To2);
        _idRoad++;
        _roads.Add(tempRoad);
        _roadSpawner.displayRoad(tempRoad);
     }
}