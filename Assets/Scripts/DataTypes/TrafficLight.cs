using System;
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
        public enum LightState { Green, Yellow, Red }

        private LightState _state;

        // return different information for cars depending on state
        // (velocity, stopping distance)
        public Tuple<int, int?> State
        {
            get
            {
                switch (_state)
                {
                    case LightState.Red:
                    {
                        return new Tuple<int, int?>(0, null);
                    }
                    case LightState.Yellow:
                    {
                        // calculates distance a car with intersection velocity will travel while light is yellow
                        return new Tuple<int, int?>(0, _yellowToRed * _section.edges.Select(e => e.speedLimit).Min());
                    }
                    case LightState.Green:
                    {
                        // returns velocity for intersection
                        return new Tuple<int, int?>(_section.edges.Select(e => e.speedLimit).Min(), null);
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        private Vertex _section { get; }
        public static TypePublisher typePublisher { get; } = new TypePublisher(Car.typePublisher);


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
        
        // counts ticks and compares to given length of each traffic light cycle
        // changes state accordingly and resets counter
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
                default:
                {
                    _state = LightState.Red;
                    break;
                }
            }
        }
    }
}