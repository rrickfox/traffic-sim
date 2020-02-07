using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace DataTypes
{
    // represents what you can tell about a road if you were to stand at one of its endpoints
    public class Edge : GameObjectData<Edge, EdgeBehaviour>
    {
        // the Vertex from which this edge originates
        public IVertex vertex = null;
        // represents how the road would look like from its other endpoint
        public Edge other { get; }
        // the cars on the outgoing side of the road
        public List<Car> cars { get; } = new List<Car>();
        public RoadShape shape { get; }
        // the coordinates of the end of the road from which you look at the road
        public RoadPoint originPoint => shape.points[0];
        public List<Lane> outgoingLanes { get; }
        // the incomingLanes of this are just the outgoingLanes of the other view
        public List<Lane> incomingLanes => other.outgoingLanes;
        public float length => shape.length;

        private GameObject prefab;

        public Edge(GameObject prefab, RoadShape shape, List<Lane> outgoingLanes, List<Lane> incomingLanes) : base(prefab)
        {
            this.prefab = prefab;
            this.shape = shape;
            this.outgoingLanes = outgoingLanes;
            other = new Edge(this, incomingLanes);

            Display();
        }

        // construct an Edge where other is already constructed
        private Edge(Edge other, List<Lane> outgoingLanes)
        {
            this.other = other;
            this.outgoingLanes = outgoingLanes;
            this.shape = other.shape.Inverse();
        }

        private void Display()
        {
            var meshVertices = new List<Vector3>();
            var uvs = new List<Vector2>();

            // calculate Vertices along the Mesh with needed offset and creating Triangles using the Vertices
            for (var i = 0; i < shape.points.Length; i++)
            {
                var p = shape.points[i];
                // offset and direction for the mesh-vertices
                var left = new Vector2(-p.forward.y, p.forward.x);
                var newPosLeft = p.position + left * ((CONSTANTS.LANE_WIDTH + CONSTANTS.LINE_WIDTH) * incomingLanes.Count + CONSTANTS.MIDDLE_LINE_WIDTH / 2 + CONSTANTS.BORDER_LINE_WIDTH);
                var newPosRight = p.position - left * ((CONSTANTS.LANE_WIDTH + CONSTANTS.LINE_WIDTH) * outgoingLanes.Count + CONSTANTS.MIDDLE_LINE_WIDTH / 2 + CONSTANTS.BORDER_LINE_WIDTH);
                meshVertices.Add(new Vector3(newPosLeft.x, CONSTANTS.ROAD_HEIGHT, newPosLeft.y));
                meshVertices.Add(new Vector3(newPosRight.x, CONSTANTS.ROAD_HEIGHT, newPosRight.y));

                // uv-coordinates
                var relativePos = i / (float)(shape.points.Length - 1);
                uvs.Add(new Vector2(0f, relativePos));
                uvs.Add(new Vector2(1f, relativePos));
            }
            
            var triangles =
                Enumerable.Range(0, shape.points.Length - 2)
                .Select(i => 2 * i)
                .Aggregate(
                    Enumerable.Empty<int>(),
                    // create Triangles from one point to the next
                    (ints, i) => ints.Concat(new []{i, i+2, i + 1, i + 1, i + 2, i + 3})
                );

            // apply Mesh and Material with adapted tiling
            gameObject.GetComponent<MeshFilter>().mesh = new Mesh
            {
                vertices = meshVertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uvs.ToArray()
            };
            var tiling = Mathf.RoundToInt(shape.length * CONSTANTS.DISTANCE_UNIT / 12f);

            var texture = GetTexture();

            gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
            gameObject.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(1, tiling));
        }

        private Texture2D GetTexture()
        {
            var widthMultiplier = 100f;
            var heightMultiplier = 100f;

            // number of lines dividing lanes in same direction
            // 0 when no lanes or one lane
            var lineCountIncoming = (incomingLanes.Count > 1) ? incomingLanes.Count - 1 : 0;
            var lineCountOutgoing = (outgoingLanes.Count > 1) ? outgoingLanes.Count - 1 : 0;

            // texture contains (left to right):
            // border, road and lines, middle, road and lines, border
            var textureWidth = Mathf.RoundToInt((CONSTANTS.MIDDLE_LINE_WIDTH + 2 * CONSTANTS.BORDER_LINE_WIDTH + CONSTANTS.LANE_WIDTH * (incomingLanes.Count + outgoingLanes.Count) + CONSTANTS.LINE_WIDTH * (lineCountIncoming + lineCountOutgoing)) * widthMultiplier);
            var textureHeight = Mathf.RoundToInt((CONSTANTS.LINE_RATIO + 1) * heightMultiplier);
            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, true);

            var colorsWithLine = GetColorRow(widthMultiplier, true);
            var colorsWithoutLine = GetColorRow(widthMultiplier, false);
            
            for(int y = 0; y < textureHeight; y++)
            {
                for(int x = 0; x < textureWidth; x++)
                {
                    // check wether y is above oder below the line segment
                    // section with line is in the middle of the texture
                    if(y < (CONSTANTS.LINE_RATIO / 2) * heightMultiplier || y >= (CONSTANTS.LINE_RATIO / 2 + 1) * heightMultiplier)
                    {
                        texture.SetPixel(x, y, colorsWithoutLine[x]);
                    } else
                    {
                        texture.SetPixel(x, y, colorsWithLine[x]);
                    }
                }
            }

            texture.Apply();

            return texture;
        }

        private Color[] GetColorRow(float widthMultiplier, bool lines)
        {
            var colorRow = new List<Color>();
            for(var i = 0; !Mathf.Approximately(CONSTANTS.BORDER_LINE_WIDTH * widthMultiplier, i); i++)
            {
                colorRow.Add(COLORS.BORDER_LINE);
            }
            for(var j = 0; j < incomingLanes.Count; j++)
            {
                if(j > 0)
                {
                    for(var i = 0; !Mathf.Approximately(CONSTANTS.LINE_WIDTH * widthMultiplier, i); i++)
                    {
                        if(lines)
                        {
                            colorRow.Add(COLORS.LINE);
                        } else
                        {
                            colorRow.Add(COLORS.ROAD);
                        }
                    }
                }
                for(var i = 0; !Mathf.Approximately(CONSTANTS.LANE_WIDTH * widthMultiplier, i); i++)
                {
                    colorRow.Add(COLORS.ROAD);
                }
            }
            for(int i = 0; !Mathf.Approximately(CONSTANTS.MIDDLE_LINE_WIDTH * widthMultiplier, i); i++)
            {
                colorRow.Add(COLORS.MIDDLE_LINE);
            }
            for(int j = 0; j < outgoingLanes.Count; j++)
            {
                if(j > 0)
                {
                    for(int i = 0; !Mathf.Approximately(CONSTANTS.LINE_WIDTH * widthMultiplier, i); i++)
                    {
                        if(lines)
                        {
                            colorRow.Add(COLORS.LINE);
                        } else
                        {
                            colorRow.Add(COLORS.ROAD);
                        }
                    }
                }
                for(int i = 0; !Mathf.Approximately(CONSTANTS.LANE_WIDTH * widthMultiplier, i); i++)
                {
                    colorRow.Add(COLORS.ROAD);
                }
            }
            for(int i = 0; !Mathf.Approximately(CONSTANTS.BORDER_LINE_WIDTH * widthMultiplier, i); i++)
            {
                colorRow.Add(COLORS.BORDER_LINE);
            }

            return colorRow.ToArray();
        }
    }

    public class EdgeBehaviour : LinkedBehaviour<Edge> { }
}
