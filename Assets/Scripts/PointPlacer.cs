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
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = new Vector3(p.x, 0.5f, p.y);
            g.transform.localScale = Vector3.one;
        }
    }
}
