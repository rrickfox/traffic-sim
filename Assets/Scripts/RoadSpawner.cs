using UnityEngine;
using System.Collections.Generic;
using System;
using DataTypes;
public class RoadSpawner : ScriptableObject
{
    private GameObject _roadPrefab;
    private List<GameObject> _roads = new List<GameObject>();

    public RoadSpawner(GameObject roadPrefab)
    {
        _roadPrefab = roadPrefab;
    }

    public void displayRoad(Road road)
    {
        float scaleLength = Vector2.Distance(road.anchors[AnchorNumber.One].position, road.anchors[AnchorNumber.Two].position); 
        float scaleWidth = (road.anchors[AnchorNumber.Two].endingLanes.Length + road.anchors[AnchorNumber.One].endingLanes.Length) * CONSTANTS.LANE_WIDTH;
        Vector2 middlePoint = (road.anchors[AnchorNumber.Two].position - road.anchors[AnchorNumber.One].position) * 0.5f + road.anchors[AnchorNumber.One].position;
        Vector3 spawnPoint = new Vector3(middlePoint.x, 0, middlePoint.y);

        Quaternion rotation = Quaternion.Euler(0, Vector2.Angle(road.anchors[AnchorNumber.One].position - middlePoint, new Vector2(1, 0)), 0);

        GameObject tempRoad = Instantiate(_roadPrefab, spawnPoint, rotation);
        tempRoad.transform.localScale = new Vector3(scaleLength, tempRoad.transform.localScale.y, scaleWidth);
        tempRoad.name = "Road_" + road.id;
        _roads.Add(tempRoad);
    }
}