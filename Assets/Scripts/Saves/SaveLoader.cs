using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static Pathfinding.Pathfinding;
using SFB;

namespace Saves
{
    public class SavePath
    {
        public static string[] paths = new string[] { "Assets/Saves/sample.yaml" };

        public static ExtensionFilter[] extension = new ExtensionFilter[] { new ExtensionFilter("Simulation Files", "yaml") };
    }

    public class SaveLoader : MonoBehaviour
    {
        private void Start()
        {
            Load(SavePath.paths[0]);
        }

        public void Load(string pathName)
        {
            var file = File.OpenText(pathName);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTagMapping("!endpoint", typeof(Serializers.EndPoint))
                .WithTagMapping("!teesection", typeof(Serializers.TeeSection))
                .WithTagMapping("!crosssection", typeof(Serializers.CrossSection))
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