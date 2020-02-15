using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using static Pathfinding.Pathfinding;

public class SampleCrosssection : MonoBehaviour
{
    public GameObject roadPrefab;
    public GameObject carPrefab;

    private List<IVertex> _vertices = new List<IVertex>();
    public void Start()
    {
        var shape_left = new RoadShape(new List<BezierCurve>(){new BezierCurve(new Vector2(-140,0), new Vector2(-75,0), new Vector2(-10,0))});
        var lanes_left_incoming = new List<Lane>{new Lane(new HashSet<LaneType>(){LaneType.LeftTurn}), new Lane(new HashSet<LaneType>(){LaneType.Through, LaneType.RightTurn})};
        var lanes_left_outgoing = new List<Lane>{new Lane(new HashSet<LaneType>(){LaneType.LeftTurn}), new Lane(new HashSet<LaneType>(){LaneType.Through, LaneType.RightTurn})};
        var left = new Edge(roadPrefab, shape_left, lanes_left_outgoing, lanes_left_incoming);
        var point_left = new EndPoint(left, carPrefab, new Frequencies(new int[]{75, 50}), new int[]{1, 1, 1});
        _vertices.Add(point_left);

        var shape_right = new RoadShape(new List<BezierCurve>(){new BezierCurve(new Vector2(140,0), new Vector2(75,0), new Vector2(10,0))});
        var lanes_right_incoming = new List<Lane>{new Lane(new HashSet<LaneType>(){LaneType.LeftTurn}), new Lane(new HashSet<LaneType>(){LaneType.Through, LaneType.RightTurn})};
        var lanes_right_outgoing = new List<Lane>{new Lane(new HashSet<LaneType>(){LaneType.LeftTurn}), new Lane(new HashSet<LaneType>(){LaneType.Through, LaneType.RightTurn})};
        var right = new Edge(roadPrefab, shape_right, lanes_right_outgoing, lanes_right_incoming);
        var point_right = new EndPoint(right, carPrefab, new Frequencies(new int[]{75, 50}), new int[]{1, 1, 1});
        _vertices.Add(point_right);

        var shape_up = new RoadShape(new List<BezierCurve>(){new BezierCurve(new Vector2(0,70), new Vector2(0,40), new Vector2(0,10))});
        var lanes_up_incoming = new List<Lane>{new Lane(new HashSet<LaneType>(){LaneType.LeftTurn}), new Lane(new HashSet<LaneType>(){LaneType.Through, LaneType.RightTurn})};
        var lanes_up_outgoing = new List<Lane>{new Lane(new HashSet<LaneType>(){LaneType.LeftTurn}), new Lane(new HashSet<LaneType>(){LaneType.Through, LaneType.RightTurn})};
        var up = new Edge(roadPrefab, shape_up, lanes_up_outgoing, lanes_up_incoming);
        var point_up = new EndPoint(up, carPrefab, new Frequencies(new int[]{75, 50}), new int[]{1, 1, 1});
        _vertices.Add(point_up);

        var shape_down = new RoadShape(new List<BezierCurve>(){new BezierCurve(new Vector2(0,-70), new Vector2(0,-40), new Vector2(0,-10))});
        var lanes_down_incoming = new List<Lane>{new Lane(new HashSet<LaneType>(){LaneType.LeftTurn}), new Lane(new HashSet<LaneType>(){LaneType.Through, LaneType.RightTurn})};
        var lanes_down_outgoing = new List<Lane>{new Lane(new HashSet<LaneType>(){LaneType.LeftTurn}), new Lane(new HashSet<LaneType>(){LaneType.Through, LaneType.RightTurn})};
        var down = new Edge(roadPrefab, shape_down, lanes_down_outgoing, lanes_down_incoming);
        var point_down = new EndPoint(down, carPrefab, new Frequencies(new int[]{75, 50}), new int[]{1, 1, 1});
        _vertices.Add(point_down);

        var section = new CrossSection(roadPrefab, up.other, right.other, down.other, left.other);
        _vertices.Add(section);

        StartPathfinding(_vertices);
    }
}