using System.Linq;
using System.Collections.Generic;
using Utility;
using UnityEngine;
using static Utility.CONSTANTS;
using static Utility.COLORS;
using UnitsNet;
using System;

namespace DataTypes
{
    public class TeeSection : Vertex
    {
        public struct TrafficLightConfig
        {
            public int total;
            public Dictionary<int, TrafficLight.Config> throughOrRight;
            public Dictionary<int, TrafficLight.Config> throughOrLeft;
            public Dictionary<int, TrafficLight.Config> leftOrRight;
            public TrafficLightConfig(int total, Dictionary<int, TrafficLight.Config> throughOrRight, Dictionary<int, TrafficLight.Config> throughOrLeft, Dictionary<int, TrafficLight.Config> leftOrRight)
            {
                this.total = total;
                this.throughOrRight = throughOrRight;
                this.throughOrLeft = throughOrLeft;
                this.leftOrRight = leftOrRight;
            }
        }
        public override GameObject prefab { get; } = ROAD_PREFAB;

        private Edge _throughOrRight { get; }
        private Edge _throughOrLeft { get; }
        private Edge _leftOrRight { get; }

        private Vector2 center;
        
        public TeeSection(Edge throughOrRight, Edge throughOrLeft, Edge leftOrRight, TrafficLightConfig sequence) : base(throughOrRight, throughOrLeft, leftOrRight)
        {
            _throughOrRight = throughOrRight;
            _throughOrRight.other.light = new TrafficLight(sequence.throughOrRight, sequence.total, this, _throughOrRight.other);
            _throughOrLeft = throughOrLeft;
            _throughOrLeft.other.light = new TrafficLight(sequence.throughOrLeft, sequence.total, this, _throughOrLeft.other);
            _leftOrRight = leftOrRight;
            _leftOrRight.other.light = new TrafficLight(sequence.leftOrRight, sequence.total, this, _leftOrRight.other);

            center = _leftOrRight.originPoint.position.closestPointOnLinesegment(_throughOrLeft.originPoint.position, _throughOrRight.originPoint.position);
            
            /*var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localPosition = new Vector3(center.x, 0, center.y);
            sphere.transform.localScale = Vector3.one * 2;
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localPosition = new Vector3(_leftOrRight.originPoint.position.x, 0, _leftOrRight.originPoint.position.y);
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localPosition = new Vector3(_throughOrLeft.originPoint.position.x, 0, _throughOrLeft.originPoint.position.y);
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localPosition = new Vector3(_throughOrRight.originPoint.position.x, 0, _throughOrRight.originPoint.position.y);*/
            
            Display();
            
            /*sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localPosition = new Vector3(_leftOrRight.originPoint.position.x, 0, _leftOrRight.originPoint.position.y);
            sphere.transform.localScale = Vector3.one * 1.5f;
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localPosition = new Vector3(_throughOrLeft.originPoint.position.x, 0, _throughOrLeft.originPoint.position.y);
            sphere.transform.localScale = Vector3.one * 1.5f;
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localPosition = new Vector3(_throughOrRight.originPoint.position.x, 0, _throughOrRight.originPoint.position.y);
            sphere.transform.localScale = Vector3.one * 1.5f;*/

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

            #endregion

            var meshVertices = new Vector3[20];
            var triangles = new int[84];
            var uvs = new Vector2[20];

            #region setUpMeshVertices
            // TeeSection is viewed with throughOrLeft Edge facing up, leftOrRight Edge facing left, throughOrRight Edge facing down
            var upLeftCorner = center + GetSectionCorner(null, _throughOrLeft);
            meshVertices[0] = new Vector3(upLeftCorner.x, ROAD_HEIGHT, upLeftCorner.y);

            var upBufferLeft = GetBufferCorner(_throughOrLeft, true);
            meshVertices[1] = new Vector3(upBufferLeft.x, ROAD_HEIGHT, upBufferLeft.y);
            var upBufferRight = GetBufferCorner(_throughOrLeft, false);
            meshVertices[2] = new Vector3(upBufferRight.x, ROAD_HEIGHT, upBufferRight.y);

            var upRightCorner = center + GetSectionCorner(_throughOrLeft, _leftOrRight);
            meshVertices[3] = new Vector3(upRightCorner.x, ROAD_HEIGHT, upRightCorner.y);

            var rightBufferLeft = GetBufferCorner(_leftOrRight, true);
            meshVertices[4] = new Vector3(rightBufferLeft.x, ROAD_HEIGHT, rightBufferLeft.y);
            var rightBufferRight = GetBufferCorner(_leftOrRight, false);
            meshVertices[5] = new Vector3(rightBufferRight.x, ROAD_HEIGHT, rightBufferRight.y);


            var rightDownCorner = center + GetSectionCorner(_leftOrRight, _throughOrRight);
            meshVertices[6] = new Vector3(rightDownCorner.x,  ROAD_HEIGHT, rightDownCorner.y);

            var downBufferLeft = GetBufferCorner(_throughOrRight, true);
            meshVertices[7] = new Vector3(downBufferLeft.x, ROAD_HEIGHT, downBufferLeft.y);
            var downBufferRight = GetBufferCorner(_throughOrRight, false);
            meshVertices[8] = new Vector3(downBufferRight.x, ROAD_HEIGHT, downBufferRight.y);

            var downLeftCorner = center + GetSectionCorner(_throughOrRight, null);
            meshVertices[9] = new Vector3(downLeftCorner.x, ROAD_HEIGHT, downLeftCorner.y);

            // set lower mesh vertices
            for (var i = 0; i < 10; i++)
                meshVertices[i + 10] = new Vector3(meshVertices[i].x, 0, meshVertices[i].z);
            #endregion

            for(int i = 0; i < 10; i++)
            {
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = meshVertices[i];
                sphere.transform.localScale = Vector3.one * 0.5f;
            }
            
            #region setTriangles
            // first triangle is middle of crosssection
            triangles[0] = 0;
            triangles[1] = 3;
            triangles[2] = 6;

            triangles[3] = 0;
            triangles[4] = 6;
            triangles[5] = 9;

            // other triangles make up the buffer zone
            var triIndex = 6;
            for(var i = 0; i < 9; i += 3)
            {
                triangles[triIndex] = i;
                triangles[triIndex + 1] = i + 1;
                triangles[triIndex + 2] = i + 2;

                triangles[triIndex + 3] = i;
                triangles[triIndex + 4] = i + 2;
                triangles[triIndex + 5] = i + 3;

                triIndex += 6;
            }

            // border around section
            for(var i = 0; i < 10; i++)
            {
                triangles[triIndex] = i;
                triangles[triIndex + 1] = 10 + i;
                triangles[triIndex + 2] = (i + 1) % 10;

                triangles[triIndex + 3] = 10 + i;
                triangles[triIndex + 4] = 10 + ((i + 1) % 10);
                triangles[triIndex + 5] = (i + 1) % 10;

                triIndex += 6;
            }
            /*
            // outsides towards edges
            for(var i = 0; i < 3; i++)
            {
                triangles[triIndex] = 3 * i + 1;
                triangles[triIndex + 1] = 12 + 3 * i + 1;
                triangles[triIndex + 2] = 12 + 3 * i + 2;

                triangles[triIndex + 3] = 3 * i + 1;
                triangles[triIndex + 4] = 12 + 3 * i + 2;
                triangles[triIndex + 5] = 3 * i + 2;

                triIndex += 6;
            }
            
            // left side of edge
            for(var i = 0; i < 3; i++)
            {
                triangles[triIndex] = 3 * i;
                triangles[triIndex + 1] = 12 + 3 * i;
                triangles[triIndex + 2] = 12 + 3 * i + 1;

                triangles[triIndex + 3] = 3 * i;
                triangles[triIndex + 4] = 12 + 3 * i + 1;
                triangles[triIndex + 5] = 3 * i + 1;

                triIndex += 6;
            }

            // right side of edge
            for(var i = 0; i < 3; i++)
            {
                triangles[triIndex] = 3 * i;
                triangles[triIndex + 1] = 3 * ((i + 3) % 4) + 2;
                triangles[triIndex + 2] = 12 + 3 * i;

                triangles[triIndex + 3] = 12 + 3 * i;
                triangles[triIndex + 4] = 3 * ((i + 3) % 4) + 2;
                triangles[triIndex + 5] = 12 + 3 * ((i + 3) % 4) + 2;

                triIndex += 6;
            }
            */
            #endregion

            #region setUvs
            // set uvs based on relative position of mesh vertices
            var bottomLeftCorner = center
                + _throughOrRight.originPoint.forward * Vector2.Distance(center, _throughOrRight.originPoint.position)
                + new Vector2(-_throughOrRight.originPoint.forward.y, _throughOrRight.originPoint.forward.x) * Mathf.Max(_throughOrRight.outgoingOffset, _throughOrLeft.incomingOffset);
            var bottomRightCorner = center
                + _throughOrRight.originPoint.forward * Vector2.Distance(center, _throughOrRight.originPoint.position)
                + _leftOrRight.originPoint.forward * Vector2.Distance(center, _leftOrRight.originPoint.position);
            var topLeftCorner = center
                + _throughOrLeft.originPoint.forward * Vector2.Distance(center, _throughOrLeft.originPoint.position)
                + new Vector2(-_throughOrRight.originPoint.forward.y, _throughOrRight.originPoint.forward.x) * Mathf.Max(_throughOrRight.outgoingOffset, _throughOrLeft.incomingOffset);
            var topRightCorner = center
                + _throughOrLeft.originPoint.forward * Vector2.Distance(center, _throughOrLeft.originPoint.position)
                + _leftOrRight.originPoint.forward * Vector2.Distance(center, _leftOrRight.originPoint.position);
            
            // upper uv coordinates
            uvs[1] = new Vector2(0, 1);
            uvs[2] = new Vector2(Vector2.Distance(new Vector2(meshVertices[2].x, meshVertices[2].z), upLeftCorner) / Vector2.Distance(upLeftCorner, upRightCorner), 1);
            // right uv coordinates
            uvs[4] = new Vector2(1, Vector2.Distance(new Vector2(meshVertices[4].x, meshVertices[4].z), bottomRightCorner) / Vector2.Distance(bottomRightCorner, upRightCorner));
            uvs[5] = new Vector2(1, Vector2.Distance(new Vector2(meshVertices[5].x, meshVertices[5].z), bottomRightCorner) / Vector2.Distance(bottomRightCorner, upRightCorner));
            // bottom uv coordinates
            uvs[7] = new Vector2(Vector2.Distance(new Vector2(meshVertices[7].x, meshVertices[7].z), bottomLeftCorner) / Vector2.Distance(bottomLeftCorner, bottomRightCorner), 0);
            uvs[8] = new Vector2(0, 0);
            
            // center uv coordinates
            uvs[0] = new Vector2(uvs[1].x, uvs[11].y);
            uvs[3] = new Vector2(uvs[2].x, uvs[4].y);
            uvs[6] = new Vector2(uvs[7].x, uvs[5].y);
            uvs[9] = new Vector2(uvs[8].x, uvs[10].y);
            
            Func<int, Vector2> GetCorner = i =>
            {
                var corner = Vector2.zero;
                switch(i)
                {
                    case 2:
                        corner = new Vector2(1, 1); break;
                    case 5:
                        corner = new Vector2(1, 0); break;
                }
                return corner;
            };
            for(var i = 2; i < 6; i += 3)
            {
                uvs[10 + i] = GetCorner(i);
                //uvs[10 + i + 1] = uvs[3 * i + 1] + (GetCorner(i) - uvs[3 * i + 1]) / 2;
                //uvs[10 + i + 2] = uvs[3 * i + 2] + (GetCorner((i + 1) % 4) - uvs[3 * i + 2]) / 2;
                uvs[10 + i + 1] = GetCorner(i);
                uvs[10 + i + 2] = GetCorner(i);
            }
            #endregion

            gameObject.GetComponent<MeshFilter>().mesh = new Mesh
            {
                vertices = meshVertices,
                triangles = triangles,
                uv = uvs
            };

            Texture texture = GetTexture();
            texture.wrapMode = TextureWrapMode.Clamp;
            gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
            gameObject.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", Vector2.one);
        }

        private Texture GetTexture()
        {
            var texture = new Texture2D(100, 100, TextureFormat.RGBA32, true);
            for(var y = 0; y < 100; y++)
                for(var x = 0; x < 100; x++)
                    texture.SetPixel(x: x, y: y, color: ROAD);
            texture.Apply();
            return texture;
        }

        #nullable enable
        // return position of corner when given to adjacent edges
        // notice, that the edges are not actually the relative left or right edges
        // rather, the 'left' edge is the one, where the other is to the left of it
        private Vector2 GetSectionCorner(Edge? leftEdge, Edge? rightEdge)
        {
            if(leftEdge != null && rightEdge != null)
                return leftEdge.originPoint.forward // offset to the left Edge
                    * (rightEdge.incomingLanes.Count * (LANE_WIDTH + LINE_WIDTH)
                        - ((rightEdge.incomingLanes.Count >= 1) ? LINE_WIDTH : 0)
                        + BORDER_LINE_WIDTH
                        + MIDDLE_LINE_WIDTH / 2f * ((rightEdge.incomingLanes.Count > 0) ?  1: -1))
                + rightEdge.originPoint.forward // offset to the right Edge
                    * (leftEdge.outgoingLanes.Count * (LANE_WIDTH + LINE_WIDTH)
                        - ((leftEdge.outgoingLanes.Count >= 1) ? LINE_WIDTH : 0)
                        + BORDER_LINE_WIDTH
                        + MIDDLE_LINE_WIDTH / 2f * ((leftEdge.outgoingLanes.Count > 0) ?  1: -1));
            else if(leftEdge != null || rightEdge == null)
            {
                return new Vector2(leftEdge!.originPoint.forward.y, -leftEdge.originPoint.forward.x)
                    * (leftEdge.outgoingLanes.Count * (LANE_WIDTH + LINE_WIDTH)
                        - ((leftEdge.outgoingLanes.Count >= 1) ? LINE_WIDTH : 0)
                        + BORDER_LINE_WIDTH
                        + MIDDLE_LINE_WIDTH / 2f * ((leftEdge.outgoingLanes.Count > 0) ?  1: -1));
            } else
            {
                return new Vector2(-rightEdge!.originPoint.forward.y, rightEdge.originPoint.forward.x)
                    * (rightEdge.incomingLanes.Count * (LANE_WIDTH + LINE_WIDTH)
                        - ((rightEdge.incomingLanes.Count >= 1) ? LINE_WIDTH : 0)
                        + BORDER_LINE_WIDTH
                        + MIDDLE_LINE_WIDTH / 2f * ((rightEdge.incomingLanes.Count > 0) ?  1: -1));
            }
            
        }
        #nullable disable

        // return position of corner including offset
        private Vector2 GetBufferCorner(Edge edge, bool left)
        {
            var leftVector =  new Vector2(-edge.originPoint.forward.y, edge.originPoint.forward.x);
            return edge.originPoint.position 
                +  (left ? leftVector : -leftVector)
                    * (LANE_WIDTH * (left ? edge.incomingLanes.Count : edge.outgoingLanes.Count) // lanes
                    + LINE_WIDTH * (((left ? edge.incomingLanes.Count : edge.outgoingLanes.Count) > 1) ? (left ? edge.incomingLanes.Count : edge.outgoingLanes.Count) - 1 : 0) // lines
                    + MIDDLE_LINE_WIDTH / 2f * (((left ? edge.incomingLanes.Count : edge.outgoingLanes.Count) > 0) ?  1: -1) // half the middle line
                    + BORDER_LINE_WIDTH); // border line
        }
    }
}