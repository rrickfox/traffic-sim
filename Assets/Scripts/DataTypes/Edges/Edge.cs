using System.Collections.Generic;
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

        public Edge(GameObject prefab, RoadShape shape, List<Lane> outgoingLanes, List<Lane> incomingLanes) : base()
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
            var vertices = new Vector3[shape.points.Length * 2];
            var uvs = new Vector2[vertices.Length];
            var triangles = new int[2 * (shape.points.Length - 1) * 3];
            var vertexIndex = 0;
            var triangleIndex = 0;

            for (int i = 0; i < shape.points.Length; i++)
            {
                var p = shape.points[i];
                var left = new Vector2(-p.forward.y, p.forward.x);
                var newPosLeft = p.position + left * CONSTANTS.LANE_WIDTH * incomingLanes.Count;
                var newPosRight = p.position - left * CONSTANTS.LANE_WIDTH * outgoingLanes.Count;
                vertices[vertexIndex] = new Vector3(newPosLeft.x, 0.025f, newPosLeft.y);
                vertices[vertexIndex + 1] = new Vector3(newPosRight.x, 0.025f, newPosRight.y);

                var relativPos = i / (float)(shape.points.Length - 1);
                uvs[vertexIndex] = new Vector2(0f, relativPos);
                uvs[vertexIndex + 1] = new Vector2(1f, relativPos);

                if (i < shape.points.Length - 1)
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

            var road = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            road.GetComponent<MeshFilter>().mesh = mesh;
            var tiling = shape.length * CONSTANTS.DISTANCE_UNIT * 0.1f;
            road.GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale("_MainTex", new Vector2(1, tiling));

            /*
            for (int i = 0; i < shape.points.Length; i++)
            {
                var roadPoint = shape.points[i];
                var spawnPoint = new Vector3(roadPoint.position.x, 0.025f, roadPoint.position.y);
                var rotation = Quaternion.Euler(0, Vector2.SignedAngle(roadPoint.forward, Vector2.right), 0);

                var roadSegment = Object.Instantiate(prefab, spawnPoint, rotation);
                roadSegment.transform.parent = transform;
                roadSegment.name = gameObject.name + "_Segment_" + i;

                var scaleWidth = (outgoingLanes.Count + incomingLanes.Count) * CONSTANTS.LANE_WIDTH;

                roadSegment.transform.localScale = new Vector3(roadSegment.transform.localScale.x, roadSegment.transform.localScale.y, scaleWidth);
            }*/
        }
    }

    public class EdgeBehaviour : LinkedBehaviour<Edge> { }
}
