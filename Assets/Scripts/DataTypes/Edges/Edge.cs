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
            var vertices = new Vector3[shape.points.Length * 2];
            var uvs = new Vector2[vertices.Length];
            var triangles = new int[2 * (shape.points.Length - 1) * 3];
            var vertexIndex = 0;
            var triangleIndex = 0;

            // calculate Vertices along the Mesh with needed offset and creating Triangles using the Vertices
            for (int i = 0; i < shape.points.Length; i++)
            {
                var p = shape.points[i];
                // offset and direction for the mesh-vertices
                var left = new Vector2(-p.forward.y, p.forward.x);
                var newPosLeft = p.position + left * CONSTANTS.LANE_WIDTH * incomingLanes.Count;
                var newPosRight = p.position - left * CONSTANTS.LANE_WIDTH * outgoingLanes.Count;
                vertices[vertexIndex] = new Vector3(newPosLeft.x, CONSTANTS.ROAD_HEIGHT, newPosLeft.y);
                vertices[vertexIndex + 1] = new Vector3(newPosRight.x, CONSTANTS.ROAD_HEIGHT, newPosRight.y);

                // uv-coordinates
                var relativePos = i / (float)(shape.points.Length - 1);
                uvs[vertexIndex] = new Vector2(0f, relativePos);
                uvs[vertexIndex + 1] = new Vector2(1f, relativePos);

                // create Triangles from one point to the next
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
            
            // apply Mesh and Material with adapted tiling
            gameObject.GetComponent<MeshFilter>().mesh = new Mesh {vertices=vertices, triangles=triangles, uv=uvs};
            var tiling = shape.length * CONSTANTS.DISTANCE_UNIT / 12f;
            gameObject.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(1, tiling));
        }
    }

    public class EdgeBehaviour : LinkedBehaviour<Edge> { }
}
