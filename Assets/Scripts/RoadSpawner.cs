using UnityEngine;
using System.Collections.Generic;
using DataTypes;
using Utility;

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
        
        var roadVisual = new GameObject("Road_" + _idRoad);
        
        for(int i = 0; i < road.shape.points.Length; i++)
        {
            var roadPoint = road.shape.points[i];
            var spawnPoint = new Vector3(roadPoint.position.x, 0.025f, roadPoint.position.y);
            var rotation = Quaternion.Euler(0, Vector2.SignedAngle(roadPoint.forward, Vector2.right), 0);

            var roadSegment = Object.Instantiate(_roadPrefab, spawnPoint, rotation);
            roadSegment.transform.parent = roadVisual.transform;
            roadSegment.name = "Road_" + _idRoad + "_Segment_" + i;

            var scaleWidth = (road.outgoingLanes.Count + road.incomingLanes.Count) * CONSTANTS.LANE_WIDTH;

            roadSegment.transform.localScale = new Vector3(roadSegment.transform.localScale.x, roadSegment.transform.localScale.y, scaleWidth);
        }
        
        _idRoad++;
        _roads.Add(roadVisual);
        return road;
    }
}