using UnityEngine;
using Utility;
using static Utility.CONSTANTS;
using static Utility.COLORS;
using System.Collections.Generic;
using System.Linq;

namespace DataTypes
{
    public class CrossSection : Vertex<CrossSection, CrossSectionBehaviour>
    {
        private Edge _up { get; }
        private Edge _right { get; }
        private Edge _down { get; }
        private Edge _left { get; }
        private Vector2 center;

        private Dictionary<RouteSegment, Dictionary<int, RoadShape>> _routes = new Dictionary<RouteSegment, Dictionary<int, RoadShape>>();

        public CrossSection(GameObject prefab, Edge up, Edge right, Edge down, Edge left)
            : base(prefab, up, right, down, left)
        {
            _up = up;
            _right = right;
            _down = down;
            _left = left;
            center = (_up.originPoint.position + _down.originPoint.position + _left.originPoint.position + _right.originPoint.position) / 4f;
            Display();
            GenerateRoute(_up, _right, _down, _left);
            GenerateRoute(_right, _down, _left, _up);
            GenerateRoute(_down, _left, _up, _right);
            GenerateRoute(_left, _up, _right, _down);
            ShowTracks();
        }

        public void ShowTracks()
        {
            /*
            LineRenderer lRend = gameObject.AddComponent<LineRenderer>();
            lRend.positionCount = (_routes[new RouteSegment(_up.other, LaneType.LeftTurn)][0].points.Length - (_routes[new RouteSegment(_up.other, LaneType.LeftTurn)][0].points.Length % 100)) / 100 + 1;
            lRend.material.color = Color.white;
            for (int i = 0; i < lRend.positionCount; i++)
                lRend.SetPosition(i, new Vector3(_routes[new RouteSegment(_up.other, LaneType.LeftTurn)][0].points[i*100].position.x, ROAD_HEIGHT  + i* 1f, _routes[new RouteSegment(_up.other, LaneType.LeftTurn)][0].points[i*100].position.y));
            lRend.SetPosition(lRend.positionCount-1,
                new Vector3(_routes[new RouteSegment(_up.other, LaneType.LeftTurn)][0].points[_routes[new RouteSegment(_up.other, LaneType.LeftTurn)][0].points.Length - 1].position.x, ROAD_HEIGHT + 1f, _routes[new RouteSegment(_up.other, LaneType.LeftTurn)][0].points[_routes[new RouteSegment(_up.other, LaneType.Through)][1].points.Length - 1].position.y));
            */
            foreach (var dic in _routes)
            {
                foreach (var shape in dic.Value)
                {
                    GameObject gO = GameObject.Instantiate(EMPTY_PREFAB, gameObject.transform);
                    LineRenderer lr = gO.AddComponent<LineRenderer>();
                    lr.positionCount = shape.Value.points.Length;
                    lr.startWidth = 0.2f;
                    lr.endWidth = 0.2f;
                    lr.startColor = Color.red;
                    lr.endColor = Color.green;
                    for (int i = 0; i < shape.Value.points.Length; i++)
                        lr.SetPosition(i, new Vector3(shape.Value.points[i].position.x, ROAD_HEIGHT + .1f, shape.Value.points[i].position.y));
                }
            }
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
            
            if(from == _up)
                if(to == _right)
                    return LaneType.LeftTurn;
                else if(to == _down)
                    return LaneType.Through;
                else // to == _left
                    return LaneType.RightTurn;
            if(from == _right)
                if(to == _down)
                    return LaneType.LeftTurn;
                else if(to == _left)
                    return LaneType.Through;
                else // to == _up
                    return LaneType.RightTurn;
            if(from == _down)
                if(to == _left)
                    return LaneType.LeftTurn;
                else if(to == _up)
                    return LaneType.Through;
                else // to == _right
                    return LaneType.RightTurn;
            else // from == _left
                if(to == _up)
                    return LaneType.LeftTurn;
                else if(to == _right)
                    return LaneType.Through;
                else // to == _down
                    return LaneType.RightTurn;
        }

        public void Display()
        {
            #region setOriginPoints
            // move originPoints to be in line with the borders of neighbouring edges
            _up.UpdateOriginPoint(center // center of section
                + _up.originPoint.forward // move-direction
                * ((_right.incomingLanes.Count > 0 && _left.outgoingLanes.Count > 0 ? // check if both edges have lanes on same side
                    (Mathf.Max(_right.incomingLanes.Count, _left.outgoingLanes.Count) * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH 
                    + MIDDLE_LINE_WIDTH / 2f 
                    + BORDER_LINE_WIDTH) // offset to the border of bigger edge
                : BORDER_LINE_WIDTH - MIDDLE_LINE_WIDTH / 2f) // offset if no lanes are on this side
                + STOP_LINE_WIDTH
                + SECTION_BUFFER_LENGTH));

            _right.UpdateOriginPoint(center
                + _right.originPoint.forward
                * ((_down.incomingLanes.Count > 0 && _up.outgoingLanes.Count > 0 ?
                    (Mathf.Max(_down.incomingLanes.Count, _up.outgoingLanes.Count) * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH
                    + MIDDLE_LINE_WIDTH / 2f
                    + BORDER_LINE_WIDTH)
                : BORDER_LINE_WIDTH - MIDDLE_LINE_WIDTH / 2f)
                + STOP_LINE_WIDTH
                + SECTION_BUFFER_LENGTH));

            _down.UpdateOriginPoint(center
                + _down.originPoint.forward
                * ((_left.incomingLanes.Count > 0 && _right.outgoingLanes.Count > 0 ?
                    (Mathf.Max(_left.incomingLanes.Count, _right.outgoingLanes.Count) * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH
                    + MIDDLE_LINE_WIDTH / 2f
                    + BORDER_LINE_WIDTH)
                : BORDER_LINE_WIDTH - MIDDLE_LINE_WIDTH / 2f)
                + STOP_LINE_WIDTH
                + SECTION_BUFFER_LENGTH));

            _left.UpdateOriginPoint(center
                + _left.originPoint.forward
                * ((_up.incomingLanes.Count > 0 && _down.outgoingLanes.Count > 0 ?
                    (Mathf.Max(_up.incomingLanes.Count, _down.outgoingLanes.Count) * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH
                    + MIDDLE_LINE_WIDTH / 2f
                    + BORDER_LINE_WIDTH)
                : BORDER_LINE_WIDTH - MIDDLE_LINE_WIDTH / 2f)
                + STOP_LINE_WIDTH
                + SECTION_BUFFER_LENGTH));
            #endregion

            var meshVertices = new Vector3[12];
            var triangles = new int[30];
            var uvs = new Vector2[12];

            #region setMeshVertices
            // set mesh Vertices in plus shape
            var leftUpCorner = center + GetSectionCorner(_left, _up);
            meshVertices[0] = new Vector3(leftUpCorner.x, ROAD_HEIGHT, leftUpCorner.y);

            var upBufferLeft = GetBufferCorner(_up, true);
            meshVertices[1] = new Vector3(upBufferLeft.x, ROAD_HEIGHT, upBufferLeft.y);
            var upBufferRight = GetBufferCorner(_up, false);
            meshVertices[2] = new Vector3(upBufferRight.x, ROAD_HEIGHT, upBufferRight.y);

            var upRightCorner = center + GetSectionCorner(_up, _right);
            meshVertices[3] = new Vector3(upRightCorner.x, ROAD_HEIGHT, upRightCorner.y);

            var rightBufferLeft = GetBufferCorner(_right, true);
            meshVertices[4] = new Vector3(rightBufferLeft.x, ROAD_HEIGHT, rightBufferLeft.y);
            var rightBufferRight = GetBufferCorner(_right, false);
            meshVertices[5] = new Vector3(rightBufferRight.x, ROAD_HEIGHT, rightBufferRight.y);


            var rightDownCorner = center + GetSectionCorner(_right, _down);
            meshVertices[6] = new Vector3(rightDownCorner.x,  ROAD_HEIGHT, rightDownCorner.y);

            var downBufferLeft = GetBufferCorner(_down, true);
            meshVertices[7] = new Vector3(downBufferLeft.x, ROAD_HEIGHT, downBufferLeft.y);
            var downBufferRight = GetBufferCorner(_down, false);
            meshVertices[8] = new Vector3(downBufferRight.x, ROAD_HEIGHT, downBufferRight.y);

            var downLeftCorner = center + GetSectionCorner(_down, _left);
            meshVertices[9] = new Vector3(downLeftCorner.x, ROAD_HEIGHT, downLeftCorner.y);

            var leftBufferLeft = GetBufferCorner(_left, true);
            meshVertices[10] = new Vector3(leftBufferLeft.x, ROAD_HEIGHT, leftBufferLeft.y);
            var leftBufferRight = GetBufferCorner(_left, false);
            meshVertices[11] = new Vector3(leftBufferRight.x, ROAD_HEIGHT, leftBufferRight.y);
            #endregion

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
            for(var i = 0; i < 12; i += 3)
            {
                triangles[triIndex] = i;
                triangles[triIndex + 1] = i + 1;
                triangles[triIndex + 2] = i + 2;

                triangles[triIndex + 3] = i;
                triangles[triIndex + 4] = i + 2;
                triangles[triIndex + 5] = (i + 3) % 12;

                triIndex += 6;
            }
            #endregion

            #region setUvs
            // set uvs based on relative position of mesh vertices
            var bottomLeftCorner = center
                + _down.originPoint.forward * Vector2.Distance(center, _down.originPoint.position)
                + _left.originPoint.forward * Vector2.Distance(center, _left.originPoint.position);
            var bottomRightCorner = center
                + _down.originPoint.forward * Vector2.Distance(center, _down.originPoint.position)
                + _right.originPoint.forward * Vector2.Distance(center, _right.originPoint.position);
            var topLeftCorner = center
                + _up.originPoint.forward * Vector2.Distance(center, _up.originPoint.position)
                + _left.originPoint.forward * Vector2.Distance(center, _left.originPoint.position);
            var topRightCorner = center
                + _up.originPoint.forward * Vector2.Distance(center, _up.originPoint.position)
                + _right.originPoint.forward * Vector2.Distance(center, _right.originPoint.position);
            
            // upper uv coordinates
            uvs[1] = new Vector2(Vector2.Distance(new Vector2(meshVertices[1].x, meshVertices[1].z), topLeftCorner) / Vector2.Distance(topLeftCorner, topRightCorner), 1);
            uvs[2] = new Vector2(Vector2.Distance(new Vector2(meshVertices[2].x, meshVertices[2].z), topLeftCorner) / Vector2.Distance(topLeftCorner, topRightCorner), 1);
            // right uv coordinates
            uvs[4] = new Vector2(1, Vector2.Distance(new Vector2(meshVertices[4].x, meshVertices[4].z), bottomRightCorner) / Vector2.Distance(bottomRightCorner, topRightCorner));
            uvs[5] = new Vector2(1, Vector2.Distance(new Vector2(meshVertices[5].x, meshVertices[5].z), bottomRightCorner) / Vector2.Distance(bottomRightCorner, topRightCorner));
            // bottom uv coordinates
            uvs[7] = new Vector2(Vector2.Distance(new Vector2(meshVertices[7].x, meshVertices[7].z), bottomLeftCorner) / Vector2.Distance(bottomLeftCorner, bottomRightCorner), 0);
            uvs[8] = new Vector2(Vector2.Distance(new Vector2(meshVertices[8].x, meshVertices[8].z), bottomLeftCorner) / Vector2.Distance(bottomLeftCorner, bottomRightCorner), 0);
            // left uv coordinates
            uvs[10] = new Vector2(0, Vector2.Distance(new Vector2(meshVertices[10].x, meshVertices[10].z), bottomLeftCorner) / Vector2.Distance(bottomLeftCorner, topLeftCorner));
            uvs[11] = new Vector2(0, Vector2.Distance(new Vector2(meshVertices[11].x, meshVertices[11].z), bottomLeftCorner) / Vector2.Distance(bottomLeftCorner, topLeftCorner));
            
            // center uv coordinates
            uvs[0] = new Vector2(uvs[1].x, uvs[11].y);
            uvs[3] = new Vector2(uvs[2].x, uvs[4].y);
            uvs[6] = new Vector2(uvs[7].x, uvs[5].y);
            uvs[9] = new Vector2(uvs[8].x, uvs[10].y);
            #endregion

            gameObject.GetComponent<MeshFilter>().mesh = new Mesh
            {
                vertices = meshVertices,
                triangles = triangles,
                uv = uvs
            };

            Texture texture = GetTexture(meshVertices);
            texture.wrapMode = TextureWrapMode.Clamp;
            gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
            gameObject.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", Vector2.one);
        }

        private void GenerateRoute(Edge edge, Edge relativeLeft,Edge oppositeEdge,Edge relativeRight)
        {
            _routes.Add(new RouteSegment(edge.other, LaneType.LeftTurn), new Dictionary<int, RoadShape>());
            _routes.Add(new RouteSegment(edge.other, LaneType.Through), new Dictionary<int, RoadShape>());
            _routes.Add(new RouteSegment(edge.other, LaneType.RightTurn), new Dictionary<int, RoadShape>());

            for (int i = edge.incomingLanes.Count - 1; i >= 0; i--)
            {
                if (edge.incomingLanes[i].types.Contains(LaneType.LeftTurn))
                {
                    var track = new List<BezierCurve>();
                    var lrDifference = Mathf.Clamp(relativeRight.outgoingLanes.Count - relativeLeft.incomingLanes.Count, 0f, Mathf.Infinity);
                    var preCurveStart = edge.other.GetAbsolutePosition(edge.length, i).position;
                    var preCurveEnd = preCurveStart - edge.originPoint.forward * (STOP_LINE_WIDTH
                            + lrDifference * LANE_WIDTH
                            + (lrDifference - 1) * (lrDifference == 0 ? 0f : LINE_WIDTH));
                    var preCurve = new BezierCurve(preCurveStart, preCurveStart, preCurveEnd);

                    var postCurveEnd = relativeLeft.GetAbsolutePosition(0f, i).position;
                    var udDifference = Mathf.Clamp(oppositeEdge.incomingLanes.Count - edge.outgoingLanes.Count, 0f, Mathf.Infinity);
                    var postCurveStart = postCurveEnd - relativeLeft.originPoint.forward * (STOP_LINE_WIDTH
                            + udDifference * LANE_WIDTH
                            + (udDifference - 1) * (udDifference == 0 ? 0f : LINE_WIDTH));
                    var postCurve = new BezierCurve(postCurveStart, postCurveStart, postCurveEnd);

                    var curveControll = center
                        + edge.other.GetAbsolutePosition(edge.length, i).position - edge.originPoint.position
                        + relativeLeft.GetAbsolutePosition(0f, i).position - relativeLeft.originPoint.position;
                    var curve = new BezierCurve(preCurveEnd, curveControll, postCurveStart);
                    track.Add(preCurve);
                    track.Add(curve);
                    track.Add(postCurve);

                    _routes[new RouteSegment(edge.other, LaneType.LeftTurn)].Add(i, new RoadShape(track));
                }

                var throughOffset = 0;
                if (edge.incomingLanes[i].types.Contains(LaneType.Through))
                {
                    var track = new List<BezierCurve>();
                    var lrDifference = Mathf.Clamp(relativeLeft.outgoingLanes.Count - relativeRight.incomingLanes.Count, 0f, Mathf.Infinity);
                    if (i >= oppositeEdge.outgoingLanes.Count && throughOffset == 0)
                        throughOffset = i + 1 - oppositeEdge.outgoingLanes.Count;
                    if (i - throughOffset < 0)
                        throw new NetworkConfigurationError("too many through Lanes");
                    var postCurveEnd = oppositeEdge.GetAbsolutePosition(0f, i - throughOffset).position;
                    var postCurveStart = postCurveEnd - oppositeEdge.originPoint.forward * (STOP_LINE_WIDTH
                        + lrDifference * LANE_WIDTH
                        + (lrDifference - 1) * (lrDifference == 0 ? 0f : LINE_WIDTH));
                    var curve1Start = edge.other.GetAbsolutePosition(edge.length, i).position;
                    var curve2Start = curve1Start + (postCurveStart - curve1Start) / 2f;
                    var curve1Controll = curve1Start - edge.originPoint.forward * (curve1Start - curve2Start).magnitude / 2f;
                    var curve2Controll = postCurveStart - oppositeEdge.originPoint.forward * (curve2Start - postCurveStart).magnitude / 2f;
                    track.Add(new BezierCurve(curve1Start, curve1Controll, curve2Start));
                    track.Add(new BezierCurve(curve2Start, curve2Controll, postCurveStart));
                    track.Add(new BezierCurve(postCurveStart, postCurveStart, postCurveEnd));

                    _routes[new RouteSegment(edge.other, LaneType.Through)].Add(i, new RoadShape(track));
                }

                if (edge.incomingLanes[i].types.Contains(LaneType.RightTurn))
                {
                    var track = new List<BezierCurve>();
                    var lrDifference = Mathf.Clamp(relativeLeft.incomingLanes.Count - relativeRight.outgoingLanes.Count, 0f, Mathf.Infinity);
                    var preCurveStart = edge.other.GetAbsolutePosition(edge.length, i).position;
                    var preCurveEnd = preCurveStart - edge.originPoint.forward * (STOP_LINE_WIDTH
                            + lrDifference * LANE_WIDTH
                            + (lrDifference - 1) * (lrDifference == 0 ? 0f : LINE_WIDTH));
                    var preCurve = new BezierCurve(preCurveStart, preCurveStart, preCurveEnd);

                    var postCurveEnd = relativeRight.GetAbsolutePosition(0f, i).position;
                    var udDifference = Mathf.Clamp(edge.incomingLanes.Count - oppositeEdge.outgoingLanes.Count, 0f, Mathf.Infinity);
                    var postCurveStart = postCurveEnd - relativeRight.originPoint.forward * (STOP_LINE_WIDTH
                            + udDifference * LANE_WIDTH
                            + (udDifference - 1) * (udDifference == 0 ? 0f : LINE_WIDTH));
                    var postCurve = new BezierCurve(postCurveStart, postCurveStart, postCurveEnd);

                    var curveControll = center
                        + edge.other.GetAbsolutePosition(edge.length, i).position - edge.originPoint.position
                        + relativeRight.GetAbsolutePosition(0f, i).position - relativeRight.originPoint.position;
                    var curve = new BezierCurve(preCurveEnd, curveControll, postCurveStart);
                    track.Add(preCurve);
                    track.Add(curve);
                    track.Add(postCurve);

                    _routes[new RouteSegment(edge.other, LaneType.RightTurn)].Add(i, new RoadShape(track));
                }
            }
        }

        private Texture GetTexture(Vector3[] meshVertices)
        {
            // calculate height and width based on origin Points of opposite edges
            var height = Mathf.RoundToInt(Vector2.Distance(_up.originPoint.position, _down.originPoint.position) * MULTIPLIER_SECTION);
            var width = Mathf.RoundToInt(Vector2.Distance(_right.originPoint.position, _left.originPoint.position) * MULTIPLIER_SECTION);
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, true);

            #region preFillTexture

            // fill texture width road, replace parts later
            for(var y = 0; y < height; y++)
                for(var x = 0; x < width; x++)
                    texture.SetPixel(x: x, y: y, color: ROAD);

            var downLeftOfRoad = (Vector2.Distance(center, _left.originPoint.position)
                - Vector2.Distance(_down.originPoint.position, 
                    new Vector2(meshVertices[8].x, meshVertices[8].z))
                + BORDER_LINE_WIDTH);
            var leftBelowRoad = (Vector2.Distance(center, _down.originPoint.position)
                - Vector2.Distance(_left.originPoint.position,
                    new Vector2(meshVertices[10].x, meshVertices[10].z))
                + BORDER_LINE_WIDTH);
            for(int x = 0; x < Mathf.RoundToInt(downLeftOfRoad * MULTIPLIER_SECTION); x++)
                for(int y = 0; y < Mathf.RoundToInt(leftBelowRoad * MULTIPLIER_SECTION); y++)
                    texture.SetPixel(x: x, y:y, color: BORDER_LINE);

            var downRightOfRoad = (Vector2.Distance(center, _right.originPoint.position)
                - Vector2.Distance(_down.originPoint.position,
                    new Vector2(meshVertices[7].x, meshVertices[7].z))
                + BORDER_LINE_WIDTH);
            var rightBelowRoad = (Vector2.Distance(center, _down.originPoint.position)
                - Vector2.Distance(_right.originPoint.position,
                    new Vector2(meshVertices[5].x, meshVertices[5].z))
                + BORDER_LINE_WIDTH);
            for(var x = width; x > width - Mathf.RoundToInt(downRightOfRoad * MULTIPLIER_SECTION); x--)
                for(var y = 0; y < Mathf.RoundToInt(rightBelowRoad * MULTIPLIER_SECTION); y++)
                    texture.SetPixel(x: x, y: y, color: BORDER_LINE);
                
            var upLeftOfRoad = (Vector2.Distance(center, _left.originPoint.position)
                - Vector2.Distance(_up.originPoint.position, 
                    new Vector2(meshVertices[1].x, meshVertices[1].z))
                + BORDER_LINE_WIDTH);
            var leftAboveRoad = (Vector2.Distance(center, _up.originPoint.position)
                - Vector2.Distance(_left.originPoint.position,
                    new Vector2(meshVertices[11].x, meshVertices[11].z))
                + BORDER_LINE_WIDTH);
            for(var x = 0; x < Mathf.RoundToInt(upLeftOfRoad * MULTIPLIER_SECTION); x++)
                for(var y = height; y > height - Mathf.RoundToInt(leftAboveRoad * MULTIPLIER_SECTION); y--)
                    texture.SetPixel(x: x, y: y, color: BORDER_LINE);
            
            var upRightOfRoad = (Vector2.Distance(center, _right.originPoint.position)
                - Vector2.Distance(_up.originPoint.position,
                    new Vector2(meshVertices[2].x, meshVertices[2].z))
                + BORDER_LINE_WIDTH);
            var rightAboveRoad = (Vector2.Distance(center, _up.originPoint.position)
                - Vector2.Distance(_right.originPoint.position,
                    new Vector2(meshVertices[4].x, meshVertices[4].z))
                + BORDER_LINE_WIDTH);
            for(var x = width; x > width - Mathf.RoundToInt(upRightOfRoad * MULTIPLIER_SECTION); x--)
                for(var y = height; y > height - Mathf.RoundToInt(rightAboveRoad * MULTIPLIER_SECTION); y--)
                    texture.SetPixel(x: x, y: y, color: BORDER_LINE);

            #endregion

            #region stopLineAndBuffer
            // construct texture from bottom up
            // stop line in _down edge
            var downStopLineRow = GetDownRow(meshVertices, true);
            for(var y = 0; y < Mathf.RoundToInt(STOP_LINE_WIDTH * MULTIPLIER_SECTION); y++)
            {
                for(var x = 0; x < width; x++)
                    texture.SetPixel(x: x, y:y, color: downStopLineRow[x]);
            }

            // buffer section in _down edge
            var downBufferRow = GetDownRow(meshVertices, false);
            for(int y = Mathf.RoundToInt(STOP_LINE_WIDTH * MULTIPLIER_SECTION); y < Mathf.RoundToInt((STOP_LINE_WIDTH + SECTION_BUFFER_LENGTH ) * MULTIPLIER_SECTION); y++)
            {
                for(var x = 0; x < width; x++)
                    texture.SetPixel(x: x, y:y, color: downBufferRow[x]);
            } 

            // stop line in _up Edge
            var upStopLineRow = GetUpRow(meshVertices, true);
            for(var y = height; y > height - Mathf.RoundToInt(STOP_LINE_WIDTH * MULTIPLIER_SECTION); y--)
            {
                for(var x = 0; x < width; x++)
                    texture.SetPixel(x: x, y: y, color: upStopLineRow[x]);
            }

            // buffer section in _up Edge
            var upBufferRow = GetUpRow(meshVertices, false);
            for(var y = height - Mathf.RoundToInt(STOP_LINE_WIDTH * MULTIPLIER_SECTION); y > height - Mathf.RoundToInt((STOP_LINE_WIDTH + SECTION_BUFFER_LENGTH) * MULTIPLIER_SECTION); y--)
            {
                for(var x = 0; x < width; x++)
                    texture.SetPixel(x: x, y: y, color: upBufferRow[x]);
            }

            // stop line for _right Edge
            var rightStopLineColumn = GetRightColumn(meshVertices, true);
            for(var x = width; x > width - Mathf.RoundToInt(STOP_LINE_WIDTH * MULTIPLIER_SECTION); x--)
            {
                for(var y = 0; y < height; y++)
                    texture.SetPixel(x: x, y: y, color: rightStopLineColumn[y]);
            }

            // buffer section for _right Edge
            var rightBufferColumn = GetRightColumn(meshVertices, false);
            for(var x = width - Mathf.RoundToInt(STOP_LINE_WIDTH * MULTIPLIER_SECTION); x > width - Mathf.RoundToInt((STOP_LINE_WIDTH + SECTION_BUFFER_LENGTH) * MULTIPLIER_SECTION); x--)
            {
                for(var y = 0; y < height; y++)
                    texture.SetPixel(x: x, y: y, color: rightBufferColumn[y]);
            }

            // stop line for _left Edge
            var leftStopLineColumn = GetLeftColumn(meshVertices, true);
            for(var x = 0; x < Mathf.RoundToInt(STOP_LINE_WIDTH * MULTIPLIER_SECTION); x++)
            {
                for(var y = 0; y < height; y++)
                    texture.SetPixel(x: x, y: y, color: leftStopLineColumn[y]);
            }

            // buffer section for _left Edge
            var leftBufferColumn = GetLeftColumn(meshVertices, false);
            for(var x = Mathf.RoundToInt(STOP_LINE_WIDTH * MULTIPLIER_SECTION); x < Mathf.RoundToInt((STOP_LINE_WIDTH + SECTION_BUFFER_LENGTH) * MULTIPLIER_SECTION); x++)
            {
                for(var y = 0; y < height; y++)
                    texture.SetPixel(x: x, y: y, color: leftBufferColumn[y]);
            }
            #endregion

            texture.Apply();

            return texture;
        }

        private Color[] GetDownRow(Vector3[] meshVertices, bool stopLine)
        {
            var row = new List<Color>();
        
            void RepeatWidth(float width, Color color) => row.AddRange(Enumerable.Range(0, Mathf.RoundToInt(width * MULTIPLIER_SECTION)).Select(x => color));

            // add a transparent Pixel, as left of road counts in distances, not absolute values
            // if LeftOfRoad is 540, it has to place pixels UP TO 540, not EXACTLY 540 pixels
            // therefore a single pixel has to be set
            row.Add(TRANSPARENT);

            // if x is left of _down
            var leftOfRoad = (Vector2.Distance(center, _left.originPoint.position)
                - Vector2.Distance(_down.originPoint.position, 
                    new Vector2(meshVertices[8].x, meshVertices[8].z)));
            RepeatWidth(leftOfRoad, TRANSPARENT);
            RepeatWidth(BORDER_LINE_WIDTH, BORDER_LINE);
            for(var j = 0; j < _down.outgoingLanes.Count; j++)
            {
                if(j > 0)
                    RepeatWidth(LINE_WIDTH, ROAD);
                RepeatWidth(LANE_WIDTH, ROAD);
            }
            if(_down.incomingLanes.Count > 0 && _down.outgoingLanes.Count > 0)
                RepeatWidth(MIDDLE_LINE_WIDTH, MIDDLE_LINE);
            for(var j = 0; j < _down.incomingLanes.Count; j++)
            {
                if(j > 0)
                    RepeatWidth(LINE_WIDTH, (stopLine ? STOP_LINE : ROAD));
                RepeatWidth(LANE_WIDTH, (stopLine ? STOP_LINE : ROAD));
            }
            RepeatWidth(BORDER_LINE_WIDTH, BORDER_LINE);
            var rightOfRoad = (Vector2.Distance(center, _right.originPoint.position)
                - Vector2.Distance(_down.originPoint.position,
                    new Vector2(meshVertices[7].x, meshVertices[7].z)));
            RepeatWidth(rightOfRoad, TRANSPARENT);

            return row.ToArray();
        }

        private Color[] GetUpRow(Vector3[] meshVertices, bool stopLine)
        {
            var row = new List<Color>();
        
            void RepeatWidth(float width, Color color) => row.AddRange(Enumerable.Range(0, Mathf.RoundToInt(width * MULTIPLIER_SECTION)).Select(x => color));

            // add a transparent Pixel, as left of road counts in distances, not absolute values
            // if LeftOfRoad is 540, it has to place pixels UP TO 540, not EXACTLY 540 pixels
            // therefore a single pixel has to be set
            row.Add(TRANSPARENT);

            // if x is left of _down
            var leftOfRoad = (Vector2.Distance(center, _left.originPoint.position)
                - Vector2.Distance(_up.originPoint.position, 
                    new Vector2(meshVertices[1].x, meshVertices[1].z)));
            RepeatWidth(leftOfRoad, TRANSPARENT);
            RepeatWidth(BORDER_LINE_WIDTH, BORDER_LINE);
            for(var j = 0; j < _up.incomingLanes.Count; j++)
            {
                if(j > 0)
                    RepeatWidth(LINE_WIDTH, (stopLine ? STOP_LINE : ROAD));
                RepeatWidth(LANE_WIDTH, (stopLine ? STOP_LINE : ROAD));
            }
            if(_up.incomingLanes.Count > 0 && _up.outgoingLanes.Count > 0)
                RepeatWidth(MIDDLE_LINE_WIDTH, MIDDLE_LINE);
            for(var j = 0; j < _up.outgoingLanes.Count; j++)
            {
                if(j > 0)
                    RepeatWidth(LINE_WIDTH, ROAD);
                RepeatWidth(LANE_WIDTH, ROAD);
            }
            RepeatWidth(BORDER_LINE_WIDTH, BORDER_LINE);
            var rightOfRoad = (Vector2.Distance(center, _right.originPoint.position)
                - Vector2.Distance(_up.originPoint.position,
                    new Vector2(meshVertices[2].x, meshVertices[2].z)));
            RepeatWidth(rightOfRoad, TRANSPARENT);

            return row.ToArray();
        }

        private Color[] GetRightColumn(Vector3[] meshVertices, bool stopLine)
        {
            var column = new List<Color>();
        
            void RepeatHeight(float height, Color color) => column.AddRange(Enumerable.Range(0, Mathf.RoundToInt(height * MULTIPLIER_SECTION)).Select(y => color));

            column.Add(TRANSPARENT);

            var belowRoad = (Vector2.Distance(center, _down.originPoint.position)
                - Vector2.Distance(_right.originPoint.position,
                    new Vector2(meshVertices[5].x, meshVertices[5].z)));
            RepeatHeight(belowRoad, TRANSPARENT);
            RepeatHeight(BORDER_LINE_WIDTH, BORDER_LINE);
            for(var j = 0; j < _right.outgoingLanes.Count; j++)
            {
                if(j > 0)
                    RepeatHeight(LINE_WIDTH, ROAD);
                RepeatHeight(LANE_WIDTH, ROAD);
            }
            if(_right.incomingLanes.Count > 0 && _right.outgoingLanes.Count > 0)
                RepeatHeight(MIDDLE_LINE_WIDTH, MIDDLE_LINE);
            for(var j = 0; j < _right.incomingLanes.Count; j++)
            {
                if(j > 0)
                    RepeatHeight(LINE_WIDTH, (stopLine ? STOP_LINE : ROAD));
                RepeatHeight(LANE_WIDTH, (stopLine ? STOP_LINE : ROAD));
            }
            RepeatHeight(BORDER_LINE_WIDTH, BORDER_LINE);
            var aboveRoad = (Vector2.Distance(center, _up.originPoint.position)
                - Vector2.Distance(_right.originPoint.position,
                    new Vector2(meshVertices[4].x, meshVertices[4].z)));
            RepeatHeight(aboveRoad, TRANSPARENT);

            return column.ToArray();
        }

        private Color[] GetLeftColumn(Vector3[] meshVertices, bool stopLine)
        {
            var column = new List<Color>();
        
            void RepeatHeight(float height, Color color) => column.AddRange(Enumerable.Range(0, Mathf.RoundToInt(height * MULTIPLIER_SECTION)).Select(y => color));

            column.Add(TRANSPARENT);

            var belowRoad = (Vector2.Distance(center, _down.originPoint.position)
                - Vector2.Distance(_left.originPoint.position,
                    new Vector2(meshVertices[10].x, meshVertices[10].z)));
            RepeatHeight(belowRoad, TRANSPARENT);
            RepeatHeight(BORDER_LINE_WIDTH, BORDER_LINE);
            for(var j = 0; j < _left.incomingLanes.Count; j++)
            {
                if(j > 0)
                    RepeatHeight(LINE_WIDTH, (stopLine ? STOP_LINE : ROAD));
                RepeatHeight(LANE_WIDTH, (stopLine ? STOP_LINE : ROAD));
            }
            if(_left.incomingLanes.Count > 0 && _left.outgoingLanes.Count > 0)
                RepeatHeight(MIDDLE_LINE_WIDTH, MIDDLE_LINE);
            for(var j = 0; j < _left.outgoingLanes.Count; j++)
            {
                if(j > 0)
                    RepeatHeight(LINE_WIDTH, ROAD);
                RepeatHeight(LANE_WIDTH, ROAD);
            }
            RepeatHeight(BORDER_LINE_WIDTH, BORDER_LINE);
            var aboveRoad = (Vector2.Distance(center, _up.originPoint.position)
                - Vector2.Distance(_left.originPoint.position,
                    new Vector2(meshVertices[11].x, meshVertices[11].z)));
            RepeatHeight(aboveRoad, TRANSPARENT);

            return column.ToArray();
        }
        
        // return position of corner when given to adjacent edges
        private Vector2 GetSectionCorner(Edge leftEdge, Edge rightEdge)
        {
            return leftEdge.originPoint.forward // offset to the left Edge
                * (rightEdge.incomingLanes.Count * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH
                    + BORDER_LINE_WIDTH
                    + MIDDLE_LINE_WIDTH / 2)
            + rightEdge.originPoint.forward // offset to the right Edge
                * (leftEdge.outgoingLanes.Count * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH
                    + BORDER_LINE_WIDTH
                    + MIDDLE_LINE_WIDTH / 2);
        }

        // return position of corner including offset
        private Vector2 GetBufferCorner(Edge edge, bool left)
        {
            var leftVector =  new Vector2(-edge.originPoint.forward.y, edge.originPoint.forward.x);
            return edge.originPoint.position 
                +  (left ? leftVector : -leftVector)
                    * (LANE_WIDTH * (left ? edge.incomingLanes.Count : edge.outgoingLanes.Count) // lanes
                    + LINE_WIDTH * (((left ? edge.incomingLanes.Count : edge.outgoingLanes.Count) > 1) ? (left ? edge.incomingLanes.Count : edge.outgoingLanes.Count) - 1 : 0) // lines
                    + MIDDLE_LINE_WIDTH / 2f // half the middle line
                    + BORDER_LINE_WIDTH // border line
                    - (((left ? edge.incomingLanes.Count : edge.outgoingLanes.Count) > 0) ? 0 : MIDDLE_LINE_WIDTH / 2f)); // subtract half the middle line if no lanes incoming
        }
    }
    
    public class CrossSectionBehaviour : VertexBehaviour<CrossSection> { }
}