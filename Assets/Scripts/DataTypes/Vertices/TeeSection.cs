using System.Linq;
using System.Collections.Generic;
using Utility;
using UnityEngine;
using static Utility.CONSTANTS;
using UnitsNet;

namespace DataTypes
{
    public class TeeSection : Vertex
    {
        public override GameObject prefab { get; } = CONSTANTS.EMPTY_PREFAB;

        private Edge _throughOrRight { get; }
        private Edge _throughOrLeft { get; }
        private Edge _leftOrRight { get; }

        private Vector2 center;
        
        public TeeSection(Edge throughOrRight, Edge throughOrLeft, Edge leftOrRight, Dictionary<TrafficLight.LightState, int> lightFrequencies)
            : base(throughOrRight, throughOrLeft, leftOrRight)
        {
            _throughOrRight = throughOrRight;
            _throughOrRight.other.light = new TrafficLight(lightFrequencies, this, TrafficLight.LightState.Green);
            _throughOrLeft = throughOrLeft;
            _throughOrLeft.other.light = new TrafficLight(lightFrequencies, this, TrafficLight.LightState.Green);
            _leftOrRight = leftOrRight;
            // calculates cycles based on perpendicular street
            if(lightFrequencies.Values.Any(freq => freq != 0)) // check if all the frequencies are 0
                _leftOrRight.other.light = new TrafficLight(lightFrequencies[TrafficLight.LightState.Yellow] + lightFrequencies[TrafficLight.LightState.Green]
                , lightFrequencies[TrafficLight.LightState.Yellow], lightFrequencies[TrafficLight.LightState.Red] - lightFrequencies[TrafficLight.LightState.Yellow], this, TrafficLight.LightState.Red);
            else
                _leftOrRight.other.light = new TrafficLight(lightFrequencies, this, TrafficLight.LightState.Green);

            center = (_throughOrRight.originPoint.position + _throughOrLeft.originPoint.position + _leftOrRight.originPoint.position) / 3f;

            routes = new Dictionary<RouteSegment, Dictionary<int, SectionTrack>>();
            GenerateRoute(_throughOrRight, null, _throughOrLeft, _leftOrRight);
            GenerateRoute(_throughOrLeft, _leftOrRight, _throughOrRight, null);
            GenerateRoute(_leftOrRight, _throughOrRight, null, _throughOrLeft);
            // TODO: toggle visibility of tracks via UI
            ShowTracks();
        }

        // returns necessary lane to go from an edge to another edge
        // throws exception if edges are not in this vertex
        // throws exception if edges are equal
        public override LaneType SubRoute(Edge comingFrom, Edge to)
        {
            var from = comingFrom.other; // Subroute gets called with the Edge facing this Vertex, therefore other must be called
            if (!edges.Contains(from)) throw new NetworkConfigurationError("From Edge not found");
            if(!edges.Contains(to)) throw new NetworkConfigurationError("To Edge not found");
            if(from == to) throw new NetworkConfigurationError("From and to are the same Edge");
            
            if(from == _leftOrRight)
                if(to == _throughOrRight)
                    return LaneType.LeftTurn;
                else // to == _throughOrLeft
                    return LaneType.RightTurn;
            if(from == _throughOrRight)
                if(to == _leftOrRight)
                    return LaneType.RightTurn;
                else // to == _throughOrLeft
                    return LaneType.Through;
            else // from == _throughOrLeft
                if(to == _leftOrRight)
                    return LaneType.LeftTurn;
                else // to == _throughOrRight
                    return LaneType.Through;
        }

        public override bool IsRoutePossible(Edge from, Edge to)
        {
            if (!edges.Contains(from)) throw new NetworkConfigurationError("From Edge not found");
            if(!edges.Contains(to)) throw new NetworkConfigurationError("To Edge not found");
            if(from == to) return false;
            return from.incomingLanes.Any(lane => lane.types.Contains(SubRoute(from.other, to)));
        }

        #nullable enable // needed for nullable Edges
        private void GenerateRoute(Edge edge, Edge? relativeLeft, Edge? oppositeEdge, Edge? relativeRight)
        {
            if(oppositeEdge != null && relativeRight != null)
            {
                // ThroughOrRight
                if(edge.incomingLanes.Any(lane => lane.types.Contains(LaneType.LeftTurn)))
                    throw new NetworkConfigurationError("TeeSection: ThroughOrRight edge contains lane which allows left turns");
                routes.Add(new RouteSegment(edge.other, LaneType.Through), new Dictionary<int, SectionTrack>());
                routes.Add(new RouteSegment(edge.other, LaneType.RightTurn), new Dictionary<int, SectionTrack>());
            } else if(oppositeEdge != null && relativeLeft != null)
            {
                // ThroughOrLeft
                if(edge.incomingLanes.Any(lane => lane.types.Contains(LaneType.RightTurn)))
                    throw new NetworkConfigurationError("TeeSection: ThroughOrLeft edge contains lane which allows right turns");
                routes.Add(new RouteSegment(edge.other, LaneType.Through), new Dictionary<int, SectionTrack>());
                routes.Add(new RouteSegment(edge.other, LaneType.LeftTurn), new Dictionary<int, SectionTrack>());
            } else if(relativeLeft != null && relativeRight != null)
            {
                // LeftOrRight
                if(edge.incomingLanes.Any(lane => lane.types.Contains(LaneType.Through)))
                    throw new NetworkConfigurationError("TeeSection: LeftOrRight edge contains lane which allows throughpassing");
                routes.Add(new RouteSegment(edge.other, LaneType.LeftTurn), new Dictionary<int, SectionTrack>());
                routes.Add(new RouteSegment(edge.other, LaneType.RightTurn), new Dictionary<int, SectionTrack>());
            } else {
                throw new NetworkConfigurationError("TeeSection has to few specified edges for route generation!");
            }






            var throughOffset = 0;

            for (int i = edge.incomingLanes.Count - 1; i >= 0; i--)
            {
                if (edge.incomingLanes[i].types.Contains(LaneType.LeftTurn))
                {
                    var track = new List<BezierCurve>();
                    var lrDifference = 0f;
                    if(relativeRight != null && relativeLeft != null)
                        lrDifference = Mathf.Clamp(relativeRight!.outgoingLanes.Count - relativeLeft!.incomingLanes.Count, 0f, Mathf.Infinity);
                    var preCurveStart = edge.other.GetAbsolutePosition(edge.length, i).position;
                    var preCurveEnd = preCurveStart - edge.originPoint.forward * (STOP_LINE_WIDTH
                            + SECTION_BUFFER_LENGTH
                            + lrDifference * LANE_WIDTH
                            + (lrDifference - 1) * (lrDifference == 0 ? 0f : LINE_WIDTH));
                    var preCurve = new BezierCurve(preCurveStart, preCurveEnd);

                    var postCurveEnd = relativeLeft!.GetAbsolutePosition(Length.Zero, i).position;
                    var udDifference = 0f;
                    if(oppositeEdge != null)
                        udDifference = Mathf.Clamp(oppositeEdge!.incomingLanes.Count - edge.outgoingLanes.Count, 0f, Mathf.Infinity);
                    var postCurveStart = postCurveEnd - relativeLeft.originPoint.forward * (STOP_LINE_WIDTH
                            + SECTION_BUFFER_LENGTH
                            + udDifference * LANE_WIDTH
                            + (udDifference - 1) * (udDifference == 0 ? 0f : LINE_WIDTH));
                    var postCurve = new BezierCurve(postCurveStart, postCurveEnd);

                    var curveControll = center
                        + preCurveStart - edge.originPoint.position
                        + postCurveEnd - relativeLeft.originPoint.position;
                    var curve = new BezierCurve(preCurveEnd, postCurveStart, curveControll);
                    track.Add(preCurve);
                    track.Add(curve);
                    track.Add(postCurve);

                    routes[new RouteSegment(edge.other, LaneType.LeftTurn)].Add(i, new SectionTrack(this, new RoadShape(track), i));
                }

                if (edge.incomingLanes[i].types.Contains(LaneType.Through))
                {
                    var track = new List<BezierCurve>();
                    var lrDifference = 0f;
                    if(relativeLeft != null && relativeRight != null)
                        lrDifference = Mathf.Clamp(relativeLeft!.outgoingLanes.Count - relativeRight!.incomingLanes.Count, 0f, Mathf.Infinity);
                    if (i >= oppositeEdge!.outgoingLanes.Count && throughOffset == 0)
                        throughOffset = i + 1 - oppositeEdge.outgoingLanes.Count;
                    if (i - throughOffset < 0)
                        throw new NetworkConfigurationError("too many through Lanes");
                    var postCurveEnd = oppositeEdge.GetAbsolutePosition(Length.Zero, i - throughOffset).position;
                    var postCurveStart = postCurveEnd - oppositeEdge.originPoint.forward * (STOP_LINE_WIDTH 
                        + SECTION_BUFFER_LENGTH
                        + lrDifference * LANE_WIDTH
                        + (lrDifference - 1) * (lrDifference == 0 ? 0f : LINE_WIDTH));
                    var curve1Start = edge.other.GetAbsolutePosition(edge.length, i).position;
                    var curve2Start = curve1Start + (postCurveStart - curve1Start) / 2f;
                    var curve1Controll = curve1Start - edge.originPoint.forward * (curve1Start - curve2Start).magnitude / 2f;
                    var curve2Controll = postCurveStart - oppositeEdge.originPoint.forward * (curve2Start - postCurveStart).magnitude / 2f;
                    track.Add(new BezierCurve(curve1Start, postCurveStart, curve1Controll, curve2Controll));
                    track.Add(new BezierCurve(postCurveStart, postCurveEnd));

                    routes[new RouteSegment(edge.other, LaneType.Through)].Add(i, new SectionTrack(this, new RoadShape(track), i - throughOffset));
                }

                if (edge.incomingLanes[i].types.Contains(LaneType.RightTurn))
                {
                    var track = new List<BezierCurve>();
                    var lrDifference = 0f;
                    if(relativeLeft != null && relativeRight != null)
                        lrDifference = Mathf.Clamp(relativeLeft!.incomingLanes.Count - relativeRight!.outgoingLanes.Count, 0f, Mathf.Infinity);
                    var preCurveStart = edge.other.GetAbsolutePosition(edge.length, i).position;
                    var preCurveEnd = preCurveStart - edge.originPoint.forward * (STOP_LINE_WIDTH
                            + SECTION_BUFFER_LENGTH
                            + lrDifference * LANE_WIDTH
                            + (lrDifference - 1) * (lrDifference == 0 ? 0f : LINE_WIDTH));
                    var preCurve = new BezierCurve(preCurveStart, preCurveEnd);

                    var postCurveEnd = new Vector2();
                    var rDifference = relativeRight!.outgoingLanes.Count - edge.incomingLanes.Count;
                    if (i + rDifference >= 0)
                        postCurveEnd = relativeRight.GetAbsolutePosition(Length.Zero, i + rDifference).position;
                    else
                        throw new NetworkConfigurationError("too many right turns");
                    var udDifference = 0f;
                    if(oppositeEdge != null)
                        udDifference = Mathf.Clamp(oppositeEdge!.outgoingLanes.Count - edge.incomingLanes.Count, 0f, Mathf.Infinity);
                    var postCurveStart = postCurveEnd - relativeRight.originPoint.forward * (STOP_LINE_WIDTH
                            + SECTION_BUFFER_LENGTH
                            + udDifference * LANE_WIDTH
                            + (udDifference - 1) * (udDifference == 0 ? 0f : LINE_WIDTH));
                    var postCurve = new BezierCurve(postCurveStart, postCurveEnd);

                    var curveControll = center
                        + preCurveStart - edge.originPoint.position
                        + postCurveEnd - relativeRight.originPoint.position;
                    var curve = new BezierCurve(preCurveEnd, postCurveStart, curveControll);
                    track.Add(preCurve);
                    track.Add(curve);
                    track.Add(postCurve);

                    routes[new RouteSegment(edge.other, LaneType.RightTurn)].Add(i, new SectionTrack(this, new RoadShape(track), i + rDifference));
                }
            }
        }

        #nullable disable
        public void ShowTracks()
        {
            foreach (var dic in routes)
            {
                foreach (var track in dic.Value)
                {
                    GameObject gO = GameObject.Instantiate(EMPTY_PREFAB, gameObject.transform);
                    LineRenderer lr = gO.AddComponent<LineRenderer>();
                    lr.positionCount = track.Value.shape.points.Length;
                    lr.startWidth = 0.2f;
                    lr.endWidth = 0.2f;
                    lr.startColor = Color.red;
                    lr.endColor = Color.green;
                    for (int i = 0; i < track.Value.shape.points.Length; i++)
                        lr.SetPosition(i, new Vector3(track.Value.shape.points[i].position.x, ROAD_HEIGHT + 0.1f, track.Value.shape.points[i].position.y));
                }
            }
        }

        public void Display()
        {
            #region setOriginPoints
            // move originPoints to be in line with the borders of neighbouring edges
            _throughOrRight.UpdateOriginPoint(center // center of section
                + _throughOrRight.originPoint.forward // move-direction
                * ((_leftOrRight.outgoingLanes.Count > 0 ? // check if edge has lanes on this side
                    (_leftOrRight.outgoingLanes.Count * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH
                    + MIDDLE_LINE_WIDTH / 2f
                    + BORDER_LINE_WIDTH) // offset to the border of edge
                : BORDER_LINE_WIDTH - MIDDLE_LINE_WIDTH / 2f) // offset if no lanes are on this side
                + STOP_LINE_WIDTH
                + SECTION_BUFFER_LENGTH));

            _throughOrLeft.UpdateOriginPoint(center // center of section
                + _throughOrLeft.originPoint.forward // move-direction
                * ((_leftOrRight.incomingLanes.Count > 0 ? // check if edge has lanes on this side
                    (_leftOrRight.incomingLanes.Count * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH
                    + MIDDLE_LINE_WIDTH / 2f
                    + BORDER_LINE_WIDTH) // offset to the border of edge
                : BORDER_LINE_WIDTH - MIDDLE_LINE_WIDTH / 2f) // offset if no lanes are on this side
                + STOP_LINE_WIDTH
                + SECTION_BUFFER_LENGTH));

            _leftOrRight.UpdateOriginPoint(center // center of section
                + _leftOrRight.originPoint.forward // move-direction
                * ((_throughOrRight.incomingLanes.Count > 0 && _throughOrLeft.outgoingLanes.Count > 0 ? // check if both edges have lanes on same side
                    (Mathf.Max(_throughOrRight.incomingLanes.Count, _throughOrLeft.outgoingLanes.Count) * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH 
                    + MIDDLE_LINE_WIDTH / 2f 
                    + BORDER_LINE_WIDTH) // offset to the border of bigger edge
                : BORDER_LINE_WIDTH - MIDDLE_LINE_WIDTH / 2f) // offset if no lanes are on this side
                + STOP_LINE_WIDTH
                + SECTION_BUFFER_LENGTH));

            #endregion setOriginPoints
        }
    }
}