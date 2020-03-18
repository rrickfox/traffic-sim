using System.Collections.Generic;
using Events;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;

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

        public void Display()
        {
            transform.parent = _edge.transform;
            
            var position = new Vector3(_edge.other.originPoint.position.x, 0, _edge.other.originPoint.position.y);
            var right = new Vector3(-_edge.other.originPoint.forward.y, 0, _edge.other.originPoint.forward.x);
            position += right * (LANE_WIDTH * _edge.outgoingLanes.Count
                + LINE_WIDTH * Mathf.Clamp(_edge.outgoingLanes.Count - 1, 0, Mathf.Infinity)
                + MIDDLE_LINE_WIDTH / 2f
                + BORDER_LINE_WIDTH
                - ((_edge.outgoingLanes.Count > 0) ? 0 : MIDDLE_LINE_WIDTH / 2f)
                + TRAFFICLIGHT_OFFSET);
            var rotation = Quaternion.LookRotation(new Vector3(_edge.other.originPoint.position.x, 0, _edge.other.originPoint.position.y), Vector3.up);

            var stand = Object.Instantiate(Resources.Load("TrafficLights/TrafficLightStand", typeof(GameObject)) as GameObject, position, rotation);
            stand.name = $"TrafficLightStand ({gameObject.GetInstanceID()})";
            stand.transform.parent = transform;

            position += Vector3.up * 3;
            var light = Object.Instantiate(Resources.Load("TrafficLights/TrafficLight", typeof(GameObject)) as GameObject, position, rotation);
            light.name = $"TrafficLight ({gameObject.GetInstanceID()})";
            light.transform.parent = transform;
        }
    }
}