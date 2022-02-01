using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static Pathfinding.Pathfinding;
using SFB;
using Events;
using Interface;
using Utility;

namespace Saves
{
    public class SaveLoader : MonoBehaviour
    {
        public static GameObject simulation;
        public static string[] paths;
        public static ExtensionFilter[] extension = { new ExtensionFilter("Simulation Files", "yaml") };

        public SimulationManager simulationManager;

        // load a path from file browser, unload loaded scene
        public void LoadPath()
        {
            paths = StandaloneFileBrowser.OpenFilePanel("Open Simulation File", "", extension, false);
            if (paths.Length == 0)
                return;
            UpdatePublisher.ResetPublisher();
            Unload();
            Load(paths[0]);
            SimulationManager.pause = false;
            simulationManager.SwitchToSimulation();
        }

        // unloads the current scene by deleting the simulation gameobject with all his temporary children
        private void Unload()
        {
            UpdatePublisher.ResetPublisher();
            Destroy(simulation);
            simulation = Instantiate(CONSTANTS.EMPTY_PREFAB);
            simulation.name = "Simulation";
        }

        // load a yaml-file
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