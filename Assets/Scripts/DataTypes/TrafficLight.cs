using System.Linq;
using Events;
using UnityEngine;

namespace DataTypes
{
    public class TrafficLight : GameObjectData
    {
        private GameObject _trafficLightPrefab { get; }
        private int _ticks { get; set; }
        private int _redToGreen { get; }
        private int _yellowToRed { get; }
        private int _greenToYellow { get; }
        private enum LightState { Green, Yellow, Red }

        private LightState _state;

        public int[] State
        {
            get
            {
                if (_state == LightState.Red)
                {
                    return new[] {0};
                }
                if (_state == LightState.Yellow)
                {
                    return new[] {0, _yellowToRed * _section.edges.Select(e => e.speedLimit).Min()};
                }
                else
                {
                    return new[] {_section.edges.Select(e => e.speedLimit).Min()};
                }
            }
        }
        
        private Vertex _section { get; }
        public static TypePublisher typePublisher { get; } = new TypePublisher();


        public TrafficLight(GameObject prefab, int red, int yellow, int green, Vertex interSection) : base(prefab)
        {
            _trafficLightPrefab = prefab;
            _redToGreen = red;
            _yellowToRed = yellow;
            _greenToYellow = green;
            _section = interSection;
            
            _publisher = new ObjectPublisher(typePublisher);
            _publisher.Subscribe(ChangeState);
        }

        public void ChangeState()
        {
            _ticks++;
            switch (_state)
            {
                case LightState.Red:
                {
                    if (_ticks % _redToGreen == 0)
                    {
                        _ticks = 0;
                        _state = LightState.Green;
                    }

                    break;
                }
                case LightState.Yellow:
                {
                    if (_ticks % _yellowToRed == 0)
                    {
                        _ticks = 0;
                        _state = LightState.Red;
                    }

                    break;
                }
                case LightState.Green:
                {
                    if (_ticks % _greenToYellow == 0)
                    {
                        _ticks = 0;
                        _state = LightState.Red;
                    }

                    break;
                }
            }
        }
    }
}