using UnityEngine;
using System.Collections.Generic;
using DataTypes;

public class RoadSpawner
{
    private GameObject _roadPrefab { get; }
    private List<GameObject> _roads { get; } = new List<GameObject>();
    private int _idRoad = 0;

    public RoadSpawner(GameObject roadPrefab)
    {
        _roadPrefab = roadPrefab;
    }

    public Edge CreateRoad(RoadShape shape, List<Lane> lanes1To2, List<Lane> lanes2To1)
    {
        var road = new Edge(shape, lanes1To2, lanes2To1);
        // Road length
        var scaleLength = Vector2.Distance(road.originPoint.position, road.other.originPoint.position); 
        // Road width
        var scaleWidth = (road.outgoingLanes.Count + road.incomingLanes.Count) * CONSTANTS.LANE_WIDTH;
        // 
        var middlePoint = (road.other.originPoint.position - road.originPoint.position) * 0.5f + road.originPoint.position;
        // Road spawnpoint
        var spawnPoint = new Vector3(middlePoint.x, 0, middlePoint.y);
        var rotation = Quaternion.Euler(0, Vector2.SignedAngle(road.other.originPoint.position - road.originPoint.position, Vector2.right), 0);
        
        var roadVisual = Object.Instantiate(_roadPrefab, spawnPoint, rotation);
        roadVisual.transform.localScale = new Vector3(scaleLength, roadVisual.transform.localScale.y, scaleWidth);
        roadVisual.name = "Road_" + _idRoad;
        
        _idRoad++;
        _roads.Add(roadVisual);
        return road;
    }
}