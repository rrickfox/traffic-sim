using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTypes;

public class PointPlacer : MonoBehaviour
{
    void Start()
    {
        var startPoint = new Vector2(-70, -35);
        var controlPoint = new Vector2(-70, 35);
        var endPoint = new Vector2(70, 35);

        var curve = new BezierCurve(startPoint, controlPoint, endPoint);
        var shape = new RoadShape(new List<BezierCurve>() {curve});

        foreach(var p in shape.points)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.transform.position = new Vector3(p.position.x, 0, p.position.y);
            g.transform.rotation = Quaternion.Euler(0, Vector2.SignedAngle(p.forward, Vector2.right), 0);
            g.transform.localScale = new Vector3(1, 0.1f, CONSTANTS.LANE_WIDTH);
        }
    }
}
