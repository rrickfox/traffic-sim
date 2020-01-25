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
    private CarSpawner _carSpawner;
    private List<List<Car>> _cars = new List<List<Car>>();
    private int _idCar = 0;
     
    public void Start()
    {
        _roadSpawner = new RoadSpawner(roadPrefab);
        _carSpawner = new CarSpawner(carPrefab, roadPrefab);
        // Point1, Point2
        Point point1 = new Point(new Vector2(-140, 0));
        Point point2 = new Point(new Vector2(140, 0));

        // Definition lanes1To2
        HashSet<LaneType> lane1To2_0_types = new HashSet<LaneType>(); 
        lane1To2_0_types.Add(LaneType.Through);
        Lane lane1To2_0 = new Lane(lane1To2_0_types);

        var lanes1To2 = new List<Lane>()
        {
            lane1To2_0
        };

        //lane2To1
        HashSet<LaneType> lane2To1_0_types = new HashSet<LaneType>();
        lane1To2_0_types.Add(LaneType.Through);
        Lane lane2To1_0 = new Lane(lane2To1_0_types);

        var lanes2To1 = new List<Lane>()
        {
            lane2To1_0
        };

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

    public void spawnCars(Road road)
    {
        if(road.anchors[AnchorNumber.Two].endingLanes.Length > 0)
        {
            if(road.anchors[AnchorNumber.One].endingLanes.Length > 0)
            {
                Direction direction = (Random.value > 0.5f) ? Direction.direction1To2 : Direction.direction2To1;
                float lane = (Direction.direction1To2 == direction) ? Mathf.Floor(Random.Range(0, road.anchors[AnchorNumber.Two].endingLanes.Length - 1)) : Mathf.Floor(Random.Range(0, road.anchors[AnchorNumber.One].endingLanes.Length - 1));
                createCar(road, lane, direction);
            } else
            {
                createCar(road, Mathf.Floor(Random.Range(0, road.anchors[AnchorNumber.Two].endingLanes.Length - 1)), Direction.direction1To2);
            }
        } else
        {
            if(road.anchors[AnchorNumber.One].endingLanes.Length > 0)
            {
                createCar(road, Mathf.Floor(Random.Range(0, road.anchors[AnchorNumber.One].endingLanes.Length - 1)), Direction.direction2To1);
            }
        }
        
    }

    public void createCar(Road road, float lane, Direction direction)
    {
        Car tempCar = new Car(_idCar, road, 0, lane, direction);
        _cars[road.id].Add(tempCar);
        _carSpawner.displayCar(tempCar);
        _idCar++;
    }
}