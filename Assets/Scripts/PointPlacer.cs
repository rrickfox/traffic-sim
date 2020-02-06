using System.Collections.Generic;
using UnityEngine;
using DataTypes;
using Utility;

public class PointPlacer : MonoBehaviour
{
    public Material material;

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

        var vertices = new Vector3[shape.points.Length * 2];
        var uvs = new Vector2[vertices.Length];
        var triangles = new int[2 * (shape.points.Length - 1) * 3];
        var vertexIndex = 0;
        var triangleIndex = 0;

        for (int i = 0; i < shape.points.Length; i++)
        {
            var p = shape.points[i];
            var left = new Vector2(-p.forward.y, p.forward.x);
            var newPosLeft = p.position + left * CONSTANTS.LANE_WIDTH;
            var newPosRight = p.position - left * CONSTANTS.LANE_WIDTH;
            vertices[vertexIndex] = new Vector3(newPosLeft.x, 0.025f, newPosLeft.y);
            vertices[vertexIndex + 1] = new Vector3(newPosRight.x, 0.025f, newPosRight.y);

            var relativPos = i / (float)(shape.points.Length-1);
            uvs[vertexIndex] = new Vector2(0f, relativPos);
            uvs[vertexIndex + 1] = new Vector2(1f, relativPos);

            if (i < shape.points.Length-1)
            {
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 2;
                triangles[triangleIndex + 2] = vertexIndex + 1;

                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + 2;
                triangles[triangleIndex + 5] = vertexIndex + 3;
            }

            vertexIndex += 2;
            triangleIndex += 6;
        }

        var mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };

        var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.GetComponent<MeshFilter>().mesh = mesh;
        var mr = g.GetComponent<MeshRenderer>();
        mr.sharedMaterial = material;
        var tiling = shape.length * CONSTANTS.DISTANCE_UNIT * 0.1f;
        mr.sharedMaterial.SetTextureScale("_MainTex", new Vector2(1, tiling));
    }
}
