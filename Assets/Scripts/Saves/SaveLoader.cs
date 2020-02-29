using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static Pathfinding.Pathfinding;

namespace Saves
{
    public class SaveLoader : MonoBehaviour
    {
        public const float VERSION = 1.0f;
        
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

            // deserialize YAML file into intermediate network serializer object structure
            var serializerNetwork = deserializer.Deserialize<Serializers.Network>(file);

            // deserialize intermediate objects to actual DataTypes objects
            var network = serializerNetwork.Deserialize();
            
            // calculate paths before the simulation starts
            StartPathfinding(network.vertices);
        }
    }
}