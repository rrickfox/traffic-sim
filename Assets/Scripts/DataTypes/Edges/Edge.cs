using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;

namespace DataTypes
{
    // represents what you can tell about a road if you were to stand at one of its endpoints
    public class Edge : GameObjectData, ITrack
    {
        public RoadPoint originPoint => shape.points[0];
        public Vertex vertex = null; // the Vertex from which this edge originates
        public Edge other { get; } // represents how the road would look like from its other endpoint
        public List<Car> cars { get; } = new List<Car>(); // the cars on the outgoing side of the road
        public List<Lane> outgoingLanes { get; }
        public List<Lane> incomingLanes => other.outgoingLanes;
        public RoadShape shape { get; }
        public float length => shape.length;
        public float speedLimit { get; } = Conversion.UnitsPerTimeStepFromKPH(120); // maximum speed of cars
        
        public Edge(GameObject prefab, RoadShape shape, List<Lane> outgoingLanes, List<Lane> incomingLanes) : base(prefab)
        {
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

            // number of lines dividing lanes in same direction
            // 0 when no lanes or one lane
            var lineCountIncoming = (incomingLanes.Count > 1) ? incomingLanes.Count - 1 : 0;
            var lineCountOutgoing = (outgoingLanes.Count > 1) ? outgoingLanes.Count - 1 : 0;

            var leftOffset = LANE_WIDTH * incomingLanes.Count
                + LINE_WIDTH * lineCountIncoming
                + MIDDLE_LINE_WIDTH / 2
                + BORDER_LINE_WIDTH;

            var rightOffset = LANE_WIDTH * outgoingLanes.Count
                + LINE_WIDTH * lineCountOutgoing
                + MIDDLE_LINE_WIDTH / 2
                + BORDER_LINE_WIDTH;

            for (var i = 0; i < shape.points.Length; i++)
            {
                var p = shape.points[i];
                // offset and direction for the mesh-vertices
                var left = new Vector2(-p.forward.y, p.forward.x);
                var newPosLeft = p.position + left * leftOffset;
                var newPosRight = p.position - left * rightOffset;
                meshVertices.Add(new Vector3(newPosLeft.x, ROAD_HEIGHT, newPosLeft.y));
                meshVertices.Add(new Vector3(newPosRight.x, ROAD_HEIGHT, newPosRight.y));
                meshVertices.Add(new Vector3(newPosLeft.x, 0, newPosLeft.y));
                meshVertices.Add(new Vector3(newPosRight.x, 0, newPosRight.y));

                // uv-coordinates
                var relativePos = i / (float)(shape.points.Length - 1);
                var relativeInnerPos = ROAD_HEIGHT / (
                    MIDDLE_LINE_WIDTH // middle line
                    + 2 * BORDER_LINE_WIDTH // borders
                    + 2 * ROAD_HEIGHT // sides
                    + LANE_WIDTH * (incomingLanes.Count + outgoingLanes.Count) // lanes
                    + LINE_WIDTH * (lineCountIncoming + lineCountOutgoing) // lines between lanes going in the same direction
                );
                uvs.Add(new Vector2(relativeInnerPos, relativePos));
                uvs.Add(new Vector2(1 - relativeInnerPos, relativePos));
                uvs.Add(new Vector2(0f, relativePos));
                uvs.Add(new Vector2(1f, relativePos));
            }
            
            var triangles =
                Enumerable.Range(0, shape.points.Length - 4)
                .Select(i => 4 * i)
                .Aggregate(
                    Enumerable.Empty<int>(),
                    // create Triangles from one point to the next
                    (ints, i) => ints.Concat(new []{i, i + 4, i + 1, i + 1, i + 4, i + 5, // middle triangles
                        i + 2, i + 6, i, i, i + 6, i + 4, // left side
                        i + 1, i + 5, i + 3, i + 3, i + 5, i + 7}) // right side
                );

            // apply Mesh and Material with adapted tiling
            gameObject.GetComponent<MeshFilter>().mesh = new Mesh
            {
                vertices = meshVertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uvs.ToArray()
            };
            var tiling = Mathf.RoundToInt(shape.length * DISTANCE_UNIT / LINE_LENGTH);

            var texture = GetTexture(tiling);

            gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
            gameObject.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(1, tiling));
        }

        private Texture2D GetTexture(int tiling)
        {
            float heightMultiplier = Mathf.Clamp(100 * tiling, 100, 500);

            // number of lines dividing lanes in same direction
            // 0 when no lanes or one lane
            var lineCountIncoming = (incomingLanes.Count > 1) ? incomingLanes.Count - 1 : 0;
            var lineCountOutgoing = (outgoingLanes.Count > 1) ? outgoingLanes.Count - 1 : 0;

            // texture contains (left to right):
            // border, road and lines, middle, road and lines, border
            var textureWidth = Mathf.RoundToInt(
                WIDTH_MULTIPLIER * (
                    MIDDLE_LINE_WIDTH // middle line
                    + 2 * BORDER_LINE_WIDTH // borders
                    + 2 * ROAD_HEIGHT // sides
                    + LANE_WIDTH * (incomingLanes.Count + outgoingLanes.Count) // lanes
                    + LINE_WIDTH * (lineCountIncoming + lineCountOutgoing) // lines between lanes going in the same direction
                )
            );
            var textureHeight = Mathf.RoundToInt((LINE_RATIO + 1) * heightMultiplier);
            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, true);

            var colorsWithLine = GetColorRow(true).ToArray();
            var colorsWithoutLine = GetColorRow(false).ToArray();
            
            for(var y = 0; y < textureHeight; y++)
            {
                for(var x = 0; x < textureWidth; x++)
                {
                    texture.SetPixel(
                        x: x,
                        y: y,
                        // check whether y is above oder below the line segment in the middle of the texture
                        color: y < (LINE_RATIO / 2) * heightMultiplier || y >= (LINE_RATIO / 2 + 1) * heightMultiplier 
                            ? colorsWithoutLine[x]
                            : colorsWithLine[x]
                    );
                }
            }

            texture.Apply();

            return texture;
        }

        private IEnumerable<Color> GetColorRow(bool lines)
        {
            IEnumerable<Color> RepeatWidth(float width, Color color) => Enumerable.Repeat(color, (int) (width * WIDTH_MULTIPLIER));
            
            IEnumerable<Color> GetLanesColorRow(int laneCount)
            {
                for(var j = 0; j < laneCount; j++)
                {
                    if(j > 0)
                        for(var i = 0; i < (int) (LINE_WIDTH * WIDTH_MULTIPLIER); i++)
                            yield return lines ? COLORS.LINE : COLORS.ROAD;
                    for(var i = 0; i < (int) (LANE_WIDTH * WIDTH_MULTIPLIER); i++)
                        yield return COLORS.ROAD;
                }
            }
            
            foreach(var color in RepeatWidth(ROAD_HEIGHT, COLORS.ROAD)) yield return color; // left side
            foreach(var color in RepeatWidth(BORDER_LINE_WIDTH, COLORS.BORDER_LINE)) yield return color; // left border
            foreach (var color in GetLanesColorRow(incomingLanes.Count)) yield return color; // incoming lanes
            foreach(var color in RepeatWidth(MIDDLE_LINE_WIDTH, COLORS.MIDDLE_LINE)) yield return color; // middle line
            foreach (var color in GetLanesColorRow(outgoingLanes.Count)) yield return color; // outgoing lanes
            foreach(var color in RepeatWidth(BORDER_LINE_WIDTH, COLORS.BORDER_LINE)) yield return color; // right border
            foreach(var color in RepeatWidth(ROAD_HEIGHT, COLORS.ROAD)) yield return color; // right side
        }

        // retrieves position and forward vector of car on road when given relative position on road and lane 
        public RoadPoint GetAbsolutePosition(float positionOnRoad, float lane)
        {
            // get first estimation of position from saved array of points
            positionOnRoad = Mathf.Clamp(positionOnRoad, 0, length);
            var index = Mathf.RoundToInt(positionOnRoad);
            var absolutePosition = shape.points[index];

            // set offset to the right to accommodate different lanes
            var perpendicularOffset = lane * (LANE_WIDTH + LINE_WIDTH) + 0.5f * LANE_WIDTH + MIDDLE_LINE_WIDTH / 2f;

            // calculate backwards vector to rotate to right facing vector using Vector2.Perpendicular()
            var inverse = absolutePosition.forward * -1;

            absolutePosition.position += Vector2.Perpendicular(inverse) * perpendicularOffset;

            return absolutePosition;
        }
    }
}
