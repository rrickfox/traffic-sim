using UnityEngine;
using System.Collections.Generic;
using DataTypes;
public class SimulationController : MonoBehaviour
{
    public GameObject roadPrefab;
    public GameObject carPrefab;

    private RoadSpawner _roadSpawner;
    private List<RoadView> _roads = new List<RoadView>();
     
    public void Start()
    {
        _roadSpawner = new RoadSpawner(roadPrefab);
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

        // lanes2To1
        HashSet<LaneType> lane2To1_0_types = new HashSet<LaneType>();
        lane1To2_0_types.Add(LaneType.Through);
        Lane lane2To1_0 = new Lane(lane2To1_0_types);

        var lanes2To1 = new List<Lane>()
        {
            lane2To1_0
        };

        // Road create..
        createRoad(pos1, pos2, lanes1To2, lanes2To1);
    }

    public void createRoad(Vector2 pos1, Vector2 pos2, List<Lane> lanes1To2, List<Lane> lanes2To1)
    {
        RoadView view = new RoadView(new RoadShape(), pos1, pos2, lanes1To2, lanes2To1);
        _roads.Add(view);
        _roadSpawner.displayRoad(view);
    }
}