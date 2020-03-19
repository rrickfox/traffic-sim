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
        public enum LightState { Green, Yellow, Red, RedYellow }
        public LightState state { get; private set; }
        public static TypePublisher typePublisher { get; } = new TypePublisher();

        private int _ticks { get; set; }
        private int _redToRedYellow { get; }
        private int _yellow { get; }
        private int _greenToYellow { get; }
        private Vertex _section { get; }
        private Edge _edge { get; }


        public TrafficLight(Dictionary<LightState, int> frequencies, Vertex intersection, LightState start, Edge edge)
        {
            state = start;
            _redToRedYellow = frequencies[LightState.Red];
            _yellow = frequencies[LightState.Yellow];
            _greenToYellow = frequencies[LightState.Green];
            _section = intersection;
            _edge = edge;
            
            _publisher = new ObjectPublisher(typePublisher);
            _publisher.Subscribe(ChangeState);
        }
        
        public TrafficLight(int red, int yellow, int green, Vertex intersection, LightState start, Edge edge)
        {
            state = start;
            _redToRedYellow = red;
            _yellow = yellow;
            _greenToYellow = green;
            _section = intersection;
            _edge = edge;
            
            _publisher = new ObjectPublisher(typePublisher);
            _publisher.Subscribe(ChangeState);
        }

        public TrafficLight WithChangedEdge(Edge edge)
        {
            return new TrafficLight(_redToRedYellow, _yellow, _greenToYellow, _section, state, edge);
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
                    if (_ticks == _redToRedYellow)
                    {
                        _ticks = 0;
                        state = LightState.RedYellow;

                        var light = transform.GetChild(1);
                        light.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Yellow");
                    }

                    break;
                }
                case LightState.Yellow:
                {
                    if (_ticks == _yellow)
                    {
                        _ticks = 0;
                        state = LightState.Red;

                        var light = transform.GetChild(1);
                        light.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/YellowOff");
                        light.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Red");
                    }

                    break;
                }
                case LightState.Green:
                {
                    if (_ticks == _greenToYellow)
                    {
                        _ticks = 0;
                        state = LightState.Yellow;
                        
                        var light = transform.GetChild(1);
                        light.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/GreenOff");
                        light.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Yellow");
                    }

                    break;
                }
                case LightState.RedYellow:
                {
                    if(_ticks == _yellow)
                    {
                        _ticks = 0;
                        state = LightState.Green;

                        var light = transform.GetChild(1);
                        light.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/RedOff");
                        light.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/YellowOff");
                        light.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Green");
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

            light.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/RedOff");
            light.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/YellowOff");
            light.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/GreenOff");

            switch(state)
            {
                case LightState.Red:
                {
                    light.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Red");
                    break;
                }
                case LightState.Yellow:
                {
                    light.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Yellow");
                    break;
                }
                case LightState.Green:
                {
                    light.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Green");
                    break;
                }
                case LightState.RedYellow:
                {
                    light.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Red");
                    light.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Yellow");
                    break;
                }
            }
        }
    }
}