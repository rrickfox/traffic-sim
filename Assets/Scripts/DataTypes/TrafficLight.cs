using System.Collections.Generic;
using Events;
using UnityEngine;
using Utility;

namespace DataTypes
{
    public class TrafficLight : GameObjectData
    {
        public override GameObject prefab { get; } = CONSTANTS.EMPTY_PREFAB;
        public enum LightState { Green, Yellow, Red }
        public LightState state { get; private set; }
        public static TypePublisher typePublisher { get; } = new TypePublisher();

        private int _ticks { get; set; }
        private int _redToGreen { get; }
        private int _yellowToRed { get; }
        private int _greenToYellow { get; }
        private Vertex _section { get; }
        private Edge _edge { get; }


        public TrafficLight(Dictionary<LightState, int> frequencies, Vertex intersection, LightState start, Edge edge)
        {
            state = start;
            _redToGreen = frequencies[LightState.Red];
            _yellowToRed = frequencies[LightState.Yellow];
            _greenToYellow = frequencies[LightState.Green];
            _section = intersection;
            _edge = edge;
            
            _publisher = new ObjectPublisher(typePublisher);
            _publisher.Subscribe(ChangeState);
        }
        
        public TrafficLight(int red, int yellow, int green, Vertex intersection, LightState start, Edge edge)
        {
            state = start;
            _redToGreen = red;
            _yellowToRed = yellow;
            _greenToYellow = green;
            _section = intersection;
            _edge = edge;
            
            _publisher = new ObjectPublisher(typePublisher);
            _publisher.Subscribe(ChangeState);
        }

        public TrafficLight WithChangedEdge(Edge edge)
        {
            return new TrafficLight(_redToGreen, _yellowToRed, _greenToYellow, _section, state, edge);
        }

        // counts ticks and compares to given length of each traffic light cycle
        // changes state accordingly and resets counter
        public void ChangeState()
        {
            _ticks++;
            switch (state)
            {
                case LightState.Red:
                {
                    if (_ticks == _redToGreen)
                    {
                        _ticks = 0;
                        state = LightState.Green;
                    }

                    break;
                }
                case LightState.Yellow:
                {
                    if (_ticks == _yellowToRed)
                    {
                        _ticks = 0;
                        state = LightState.Red;
                    }

                    break;
                }
                case LightState.Green:
                {
                    if (_ticks == _greenToYellow)
                    {
                        _ticks = 0;
                        state = LightState.Red;
                    }

                    break;
                }
            }
        }
    }
}