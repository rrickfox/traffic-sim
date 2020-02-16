using UnityEngine;
using static Utility.CONSTANTS;
using static Utility.COLORS;
using System.Collections.Generic;

namespace DataTypes
{
    public class CrossSection : Vertex<CrossSection, CrossSectionBehaviour>
    {
        private Edge _up { get; }
        private Edge _right { get; }
        private Edge _down { get; }
        private Edge _left { get; }
        private Vector2 center;

        public CrossSection(GameObject prefab, Edge up, Edge right, Edge down, Edge left)
            : base(prefab, up, right, down, left)
        {
            _up = up;
            _right = right;
            _down = down;
            _left = left;
            center = (_up.originPoint.position + _down.originPoint.position + _left.originPoint.position + _right.originPoint.position) / 4f;
            Display();
        }

        public CrossSection(Edge up, Edge right, Edge down, Edge left) : this(EMPTY_PREFAB, up, right, down, left) {}

        // returns necessary lane to go from an edge to another edge
        // throws exception if edges are not in this vertex
        // throws exception if edges are equal
        public override LaneType SubRoute(Edge from, Edge to)
        {
            if(this.edges.Contains(from) && this.edges.Contains(to))
                if(this._up.Equals(from))
                    if(this._up.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(this._right.Equals(to))
                        return LaneType.LeftTurn;
                    else if(this._down.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
                else if(this._right.Equals(from))
                    if(this._right.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(this._down.Equals(to))
                        return LaneType.LeftTurn;
                    else if(this._left.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
                else if(this._down.Equals(from))
                    if(this._down.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(this._left.Equals(to))
                        return LaneType.LeftTurn;
                    else if(this._up.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
                else
                    if(this._left.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(this._up.Equals(to))
                        return LaneType.LeftTurn;
                    else if(this._right.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
            else
                throw new System.Exception("Edges not found");
        }

        public void Display()
        {
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

            var meshVertices = new Vector3[8];
            var triangles = new int[30];
            var uvs = new Vector2[8];

            meshVertices[0] = new Vector3(_left.originPoint.position.x + _up.originPoint.position.x - center.x,
                0f,
                _left.originPoint.position.y + _up.originPoint.position.y - center.y);
            meshVertices[1] = new Vector3(_right.originPoint.position.x + _up.originPoint.position.x - center.x,
                0f,
                _right.originPoint.position.y + _up.originPoint.position.y - center.y);
            meshVertices[2] = new Vector3(_right.originPoint.position.x + _down.originPoint.position.x - center.x,
                0f,
                _right.originPoint.position.y + _down.originPoint.position.y - center.y);
            meshVertices[3] = new Vector3(_left.originPoint.position.x + _down.originPoint.position.x - center.x,
                0f,
                _left.originPoint.position.y + _down.originPoint.position.y - center.y);
            meshVertices[4] = meshVertices[0] + new Vector3(0f, ROAD_HEIGHT, 0f);
            meshVertices[5] = meshVertices[1] + new Vector3(0f, ROAD_HEIGHT, 0f);
            meshVertices[6] = meshVertices[2] + new Vector3(0f, ROAD_HEIGHT, 0f);
            meshVertices[7] = meshVertices[3] + new Vector3(0f, ROAD_HEIGHT, 0f);

            var triIndex = 0;
            for (int i = 0; i < 5; i++)
            {
                if(i == 4)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + 1;
                    triangles[triIndex + 2] = i + 2;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 2;
                    triangles[triIndex + 5] = i + 3;
                }
                else {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = (i + 1) % 4;
                    triangles[triIndex + 2] = i + 4;

                    triangles[triIndex + 3] = i + 1;
                    triangles[triIndex + 4] = (i + 1) % 4 + 4;
                    triangles[triIndex + 5] = i + 4;
                    triIndex += 6;
                }
            }

            uvs[0] = new Vector2(0f, 1f);
            uvs[1] = new Vector2(1f, 1f);
            uvs[2] = new Vector2(1f, 0f);
            uvs[3] = new Vector2(0f, 0f);
            uvs[4] = new Vector2(ROAD_HEIGHT / (2 * ROAD_HEIGHT + (meshVertices[4] - meshVertices[5]).magnitude),
                1 - ROAD_HEIGHT / (2 * ROAD_HEIGHT + (meshVertices[4] - meshVertices[7]).magnitude));
            uvs[5] = new Vector2(1f - uvs[4].x, uvs[4].y);
            uvs[6] = new Vector2(1f - uvs[4].x, 1f - uvs[4].y);
            uvs[7] = new Vector2(uvs[4].x, 1f - uvs[4].y);

            gameObject.GetComponent<MeshFilter>().mesh = new Mesh
            {
                vertices = meshVertices,
                triangles = triangles,
                uv = uvs
            };

            Texture texture = GetTexture(Mathf.RoundToInt(2 * ROAD_HEIGHT + (meshVertices[4] - meshVertices[5]).magnitude),
                Mathf.RoundToInt(2 * ROAD_HEIGHT + (meshVertices[4] - meshVertices[7]).magnitude));
            gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
        }

        private Texture GetTexture(int width, int height)
        {
            var texture = new Texture2D(width, height, TextureFormat.ARGB32, true);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x: x, y: y, color: ROAD);
                }
            }

            texture.Apply();

            return texture;
        }
    }
    
    public class CrossSectionBehaviour : VertexBehaviour<CrossSection> { }
}