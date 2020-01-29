using UnityEngine;
using System.Collections.Generic;
using System;
using DataTypes;
public class RoadSpawner : ScriptableObject
{
    private GameObject _roadPrefab;
    private List<GameObject> _roads = new List<GameObject>();
    private int _idRoad = 0;

    public RoadSpawner(GameObject roadPrefab)
    {
        _roadPrefab = roadPrefab;
    }

    public void DisplayRoad(RoadView view)
    {
        // Road length
        float scaleLength = Vector2.Distance(view.position, view.other.position); 
        // Road width
        float scaleWidth = (view.outgoingLanes.Count + view.incomingLanes.Count) * CONSTANTS.LANE_WIDTH;
        // 
        Vector2 middlePoint = (view.other.position - view.position) * 0.5f + view.position;
        // Road spawnpoint
        Vector3 spawnPoint = new Vector3(middlePoint.x, 0, middlePoint.y);

        Quaternion rotation = Quaternion.Euler(0, Vector2.SignedAngle(view.other.position - view.position, Vector2.right), 0);

        GameObject roadVisual = Instantiate(_roadPrefab, spawnPoint, rotation);
        roadVisual.transform.localScale = new Vector3(scaleLength, roadVisual.transform.localScale.y, scaleWidth);
        roadVisual.name = "Road_" + _idRoad;
        _idRoad++;
        _roads.Add(roadVisual);
    }
}