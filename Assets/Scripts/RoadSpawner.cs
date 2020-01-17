using UnityEngine;
using System.Collections.Generic;
using System;
using DataTypes;
public class RoadSpawner : ScriptableObject
{
    private CONSTANTS _constants = new CONSTANTS();
    private GameObject _roadPrefab;
    private List<GameObject> _roads = new List<GameObject>();

    public RoadSpawner(GameObject roadPrefab)
    {
        _roadPrefab = roadPrefab;
    }

    public void displayRoad(Road road)
    {
        float scaleLength = Vector2.Distance(road.node1.position, road.node2.position);
        float scaleWidth = (road.lanes1To2 + road.lanes2To1) * CONSTANTS.LANE_WIDTH;
        Vector2 middlePoint = (road.node2.position - road.node1.position) * 0.5f + road.node1.position;
        Vector3 spawnPoint = new Vector3(middlePoint.x, 0, middlePoint.y);

        Quaternion rotation = Quaternion.Euler(0, Vector2.Angle(road.node1.position - middlePoint, new Vector2(1, 0)), 0);

        GameObject tempRoad = Instantiate(_roadPrefab, spawnPoint, rotation);
        tempRoad.transform.localScale = new Vector3(scaleLength, tempRoad.transform.localScale.y, scaleWidth);
        tempRoad.name = "Road_" + road.id;
        _roads.Add(tempRoad);
    }
}