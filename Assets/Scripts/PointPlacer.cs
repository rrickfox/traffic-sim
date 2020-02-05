using System.Collections.Generic;
using UnityEngine;
using DataTypes;
using Utility;

public class PointPlacer : MonoBehaviour
{
    void Start()
    {
        var p0_s = new Vector2(-140, -70);
        var p0_c = new Vector2(-140, 0);
        var p0_e = new Vector2(0, 0);

        var p1_s = new Vector2(0, 0);
        var p1_c = new Vector2(140, 0);
        var p1_e = new Vector2(140, 70);
        
        var curve_0 = new BezierCurve(p0_s, p0_c, p0_e);
        var curve_1 = new BezierCurve(p1_s, p1_c, p1_e);
        var shape = new RoadShape(new List<BezierCurve> {curve_0, curve_1});

        foreach(var p in shape.points)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.transform.position = new Vector3(p.position.x, 0, p.position.y);
            g.transform.rotation = Quaternion.Euler(0, Vector2.SignedAngle(p.forward, Vector2.right), 0);
            g.transform.localScale = new Vector3(CONSTANTS.DISTANCE_UNIT, 0.1f, CONSTANTS.LANE_WIDTH);
        }
    }
}
