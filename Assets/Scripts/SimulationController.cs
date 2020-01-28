using UnityEngine;
using System.Collections.Generic;
using DataTypes;
//ruft Autos zur Bewegung auf
//-> hat einzige Update-Funktion ("interne Zeit")
//erstellt Straße (vorläufig)
public class SimulationController : MonoBehaviour
{
    public GameObject roadPrefab;
    public GameObject carPrefab;

    private RoadSpawner _roadSpawner;
<<<<<<< HEAD
    private List<Road> _roads = new List<Road>();
    private int _idRoad = 0;
    private CarSpawner _carSpawner;
=======
    private List<RoadView> _roads = new List<RoadView>();
>>>>>>> develop
    private List<List<Car>> _cars = new List<List<Car>>();
    private int _idCar = 0;
     
    public void Start()
    {
        _roadSpawner = new RoadSpawner(roadPrefab);
        _carSpawner = new CarSpawner(carPrefab, roadPrefab);

<<<<<<< HEAD
        Vector2 pos1 = new Vector2(-140, 0);
        Vector2 pos2 = new Vector2(140, 0);
        createRoad(pos1, pos2, 1, 0);
        createCar(_roads[0], 0, Direction.direction1To2);
    }

    public void spawnCars(Road road)
    {
        if(road.lanes1To2 > 0)
        {
            if(road.lanes2To1 > 0)
            {
                Direction direction = (Random.value > 0.5f) ? Direction.direction1To2 : Direction.direction2To1;
                float lane = (Direction.direction1To2 == direction) ? Mathf.Floor(Random.Range(0, road.lanes1To2 - 1)) : Mathf.Floor(Random.Range(0, road.lanes2To1 - 1));
                createCar(road, lane, direction);
            } else
            {
                createCar(road, Mathf.Floor(Random.Range(0, road.lanes1To2 - 1)), Direction.direction1To2);
            }
        } else
        {
            if(road.lanes2To1 > 0)
            {
                createCar(road, Mathf.Floor(Random.Range(0, road.lanes2To1 - 1)), Direction.direction2To1);
            }
        }
        
    }

    public void createRoad(Vector2 pos1, Vector2 pos2, int lanes1To2, int lanes2To1)
    {
        Node node1 = new Node(pos1, lanes1To2, lanes2To1);
        Node node2 = new Node(pos2, lanes2To1, lanes1To2);
        Road tempRoad = new Road(_idRoad, node1, node2, lanes1To2, lanes2To1);
        _roads.Add(tempRoad);
        _cars.Add(new List<Car>());
        _roadSpawner.displayRoad(tempRoad);
        _idRoad++;
    }

    public void createCar(Road road, float lane, Direction direction)
    {
        Car tempCar = new Car(_idCar, road, 0, lane, direction);
        _cars[road.id].Add(tempCar);
        _carSpawner.displayCar(tempCar);
        _idCar++;
    }
}
=======
        // Point1, Point2
        Vector2 pos1 = new Vector2(-140, 0);
        Vector2 pos2 = new Vector2(140, 0);

        // Definition lanes1To2
        HashSet<LaneType> lane1To2_0_types = new HashSet<LaneType>(); 
        lane1To2_0_types.Add(LaneType.Through);
        Lane lane1To2_0 = new Lane(lane1To2_0_types);

        var lanes1To2 = new List<Lane>()
        {
            lane1To2_0
        };

        // Definition lanes2To1
        HashSet<LaneType> lane2To1_0_types = new HashSet<LaneType>();
        lane1To2_0_types.Add(LaneType.Through);
        Lane lane2To1_0 = new Lane(lane2To1_0_types);

        var lanes2To1 = new List<Lane>()
        {
            lane2To1_0
        };

        // Road create..
        CreateRoad(pos1, pos2, lanes1To2, lanes2To1);
    }

    public void CreateRoad(Vector2 pos1, Vector2 pos2, List<Lane> lanes1To2, List<Lane> lanes2To1)
    {
        RoadView view = new RoadView(new RoadShape(), pos1, pos2, lanes1To2, lanes2To1);
        _roads.Add(view);
        _roadSpawner.DisplayRoad(view);
    }
}
>>>>>>> develop
