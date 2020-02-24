using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Saves
{
    public class SaveLoader : MonoBehaviour
    {
        public const float VERSION = 1;
        
        private void Start()
        {
            Load("Assets/Saves/sample.yaml");
        }

        public void Load(string pathName)
        {
            var file = File.OpenText(pathName);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTagMapping("!endpoint", typeof(Serializers.EndPoint))
                .Build();

            var network = deserializer.Deserialize<Serializers.Network>(file);
        }
    }
    
    namespace Serializers
    {
        public class Network
        {
            public float version { get; set; }
            public Edges edges { get; set; }
            public Vertices vertices { get; set; }
        }
        
        public class Edges : Dictionary<int, Edge> { }

        public class Edge
        {
            public Shape shape { get; set; }
            public Lanes outgoingLanes { get; set; }
            public Lanes incomingLanes { get; set; }
        }
        
        public class Shape : List<BezierCurve> { }

        public class BezierCurve
        {
            public Point2D start { get; set; }
            public Point2D control { get; set; }
            public Point2D end { get; set; }
        }

        public class Point2D
        {
            public float x { get; set; }
            public float y { get; set; }
        }
        
        public class Lanes : HashSet<Lane> { }
        
        public class Lane : List<DataTypes.LaneType> { }
        
        public class Vertices : List<Vertex> { }

        public class Vertex
        {
            public int[] frequencies { get; set; }
        }

        public class EndPoint : Vertex
        {
            public string edge { get; set; }
        }
    }
}