using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;

namespace Saves
{
    namespace Serializers
    {
        public class Network
        {
            public float version { get; set; }
            public Edges edges { get; set; }
            public Vertices vertices { get; set; }

            public DataTypes.Network Deserialize()
            {
                if (version != SaveLoader.VERSION)
                    throw new NetworkConfigurationError(
                        "The version of the configuration file does not match the version of this build"
                    );

                var edgesLookup = edges.Deserialize();
                return new DataTypes.Network(vertices.Deserialize(edgesLookup), edgesLookup.Values.ToList());
            }
        }

        public class Edges : Dictionary<int, Edge>
        {
            public Dictionary<int, DataTypes.Edge> Deserialize()
                => this
                    .Select(kvp => new KeyValuePair<int, DataTypes.Edge>(kvp.Key, kvp.Value.Deserialize()))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public class Edge
        {
            public Shape shape { get; set; }
            public Lanes outgoingLanes { get; set; }
            public Lanes incomingLanes { get; set; }

            public DataTypes.Edge Deserialize()
                => new DataTypes.Edge(
                    ROAD_PREFAB,
                    shape.Deserialize(),
                    outgoingLanes.Deserialize(),
                    incomingLanes.Deserialize()
                );
        }

        public class Shape : List<BezierCurve>
        {
            public DataTypes.RoadShape Deserialize()
                => new DataTypes.RoadShape(this.Select(curve => curve.Deserialize()).ToList());
        }

        public class BezierCurve
        {
            public Point2D start { get; set; }
            public Point2D control { get; set; }
            public Point2D end { get; set; }

            public DataTypes.BezierCurve Deserialize()
                => new DataTypes.BezierCurve(start.Deserialize(), control.Deserialize(), end.Deserialize());
        }

        public class Point2D
        {
            public float x { get; set; }
            public float y { get; set; }

            public Vector2 Deserialize() => new Vector2(x, y);
        }

        public class Lanes : List<Lane>
        {
            public List<DataTypes.Lane> Deserialize()
                => this.Select(lane => lane.Deserialize()).ToList();
        }

        public class Lane : HashSet<DataTypes.LaneType>
        {
            public DataTypes.Lane Deserialize() => new DataTypes.Lane(this.ToHashSet());
        }

        public class Vertices : List<IVertex<DataTypes.IVertex>>
        {
            public List<DataTypes.IVertex> Deserialize(Dictionary<int, DataTypes.Edge> verticesLookup)
                => this.Select(vertex => vertex.Deserialize(verticesLookup)).ToList();
        }

        public interface IVertex<out TDeserialized>
            where TDeserialized : DataTypes.IVertex
        {
            TDeserialized Deserialize(Dictionary<int, DataTypes.Edge> verticesLookup);
        }

        public class EndPoint : IVertex<DataTypes.EndPoint>
        {
            public string edge { get; set; }
            public Frequencies frequencies { get; set; }
            public Weights weights { get; set; }

            public DataTypes.EndPoint Deserialize(Dictionary<int, DataTypes.Edge> verticesLookup)
            {
                if (string.IsNullOrEmpty(edge))
                    throw new NetworkConfigurationError("Some EndPoint's edge is empty");

                if (!int.TryParse(string.Concat(edge.SkipLast(1)), out var key))
                    throw new NetworkConfigurationError($"Some EndPoint's edge is not an integer ({edge})");

                if (!verticesLookup.TryGetValue(key, out var maybeEdge))
                    throw new NetworkConfigurationError($"Some EndPoint refers to a non-existent edge ({edge})");

                DataTypes.Edge actualEdge;
                switch (edge.Last())
                {
                    case 'a': actualEdge = maybeEdge; break;
                    case 'b': actualEdge = maybeEdge.other; break;
                    default: throw new NetworkConfigurationError(
                        $"Some EndPoint's edge does not specify which edge variant it is ({edge})"
                        );
                }

                return new DataTypes.EndPoint(actualEdge, CAR_PREFAB, frequencies.Deserialize(), weights.Deserialize());
            }
        }

        public class Frequencies : List<int>
        {
            public DataTypes.Frequencies Deserialize() => new DataTypes.Frequencies(ToArray());
        }

        public class Weights : List<int>
        {
            public int[] Deserialize() => ToArray();
        }
    }
}