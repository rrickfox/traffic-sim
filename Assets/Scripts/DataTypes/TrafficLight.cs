using System.Linq;
using System.Runtime.Versioning;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public class TrafficLight : GameObjectData
    {
        public struct Config
        {
            public List<LaneType> types;
            public LightState state;
        }
        public override GameObject prefab { get; } = CONSTANTS.EMPTY_PREFAB;
        public enum LightState { Green, Yellow, Red, RedYellow }
        public LightState state { get; private set; }
        private Dictionary<int, Config> _sequence { get; }
        private int _totalTicks { get; }
        public Dictionary<LaneType, LightState> states { get; private set; }
            = new Dictionary<LaneType, LightState>{{LaneType.LeftTurn, LightState.Red}, {LaneType.Through, LightState.Red}, {LaneType.RightTurn, LightState.Red}};
        public static TypePublisher typePublisher { get; } = new TypePublisher();

        private int _ticks { get; set; } = 0;
        private int _redToRedYellow { get; }
        private int _yellow { get; }
        private int _greenToYellow { get; }
        private Vertex _section { get; }
        private Edge _edge { get; }

        public TrafficLight(Dictionary<int, Config> sequence, int totalTicks, Vertex intersection, Edge edge)
        {
            _sequence = sequence;
            _totalTicks = totalTicks;
            _section = intersection;
            _edge = edge;

            if (_sequence.ContainsKey(0))
            {
                var startState = _sequence[0].state;
                _sequence[0].types.ForEach(type => states[type] = startState);
            }

            var publisher = new ObjectPublisher(typePublisher);
            publisher.Subscribe(ChangeState);
            _allPublishers.Add(publisher);
        }


        public TrafficLight(Dictionary<LightState, int> frequencies, Vertex intersection, LightState start, Edge edge)
        {
            state = start;
            _redToRedYellow = frequencies[LightState.Red];
            _yellow = frequencies[LightState.Yellow];
            _greenToYellow = frequencies[LightState.Green];
            _section = intersection;
            _edge = edge;
            
            var publisher = new ObjectPublisher(typePublisher);
            publisher.Subscribe(ChangeState);
            _allPublishers.Add(publisher);
        }
        
        public TrafficLight(int red, int yellow, int green, Vertex intersection, LightState start, Edge edge)
        {
            state = start;
            _redToRedYellow = red;
            _yellow = yellow;
            _greenToYellow = green;
            _section = intersection;
            _edge = edge;
            
            var publisher = new ObjectPublisher(typePublisher);
            publisher.Subscribe(ChangeState);
            _allPublishers.Add(publisher);
        }

        public TrafficLight WithChangedEdge(Edge edge)
        {
            return new TrafficLight(_redToRedYellow, _yellow, _greenToYellow, _section, state, edge);
        }

        private void UpdateVisuals() {
            if (_sequence.Values.Any(config => config.types.Count != 3))
            {
                // at some point not all are the same
            }
            else
            {
                switch(states[LaneType.Through])
                {
                    case LightState.Green:
                    {
                        // previous is red-yellow, so turn off both and turn on green
                        var light = transform.GetChild(1);
                        light.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/RedOff");
                        light.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/YellowOff");
                        light.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Green");
                        var flatLight = transform.GetChild(1);
                        flatLight.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/RedOff");
                        flatLight.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/YellowOff");
                        flatLight.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Green");
                        break;
                    }
                    case LightState.Yellow:
                    {
                        // turn of green, turn on yellow
                        var light = transform.GetChild(1);
                        light.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/GreenOff");
                        light.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Yellow");
                        var flatLight = transform.GetChild(1);
                        flatLight.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/GreenOff");
                        flatLight.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Yellow");
                        break;
                    }
                    case LightState.Red:
                    {
                        // turn of yellow, turn on red
                        var light = transform.GetChild(1);
                        light.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/YellowOff");
                        light.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Red");
                        var flatLight = transform.GetChild(1);
                        flatLight.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/YellowOff");
                        flatLight.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Red");
                        break;
                    }
                    case LightState.RedYellow:
                    {
                        // only add yellow, red is already active
                        var light = transform.GetChild(1);
                        light.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Yellow");
                        var flatLight = transform.GetChild(1);
                        flatLight.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/Yellow");
                        break;
                    }
                }
            }
        }

        // counts ticks and compares to given length of each traffic light cycle
        // changes state accordingly and resets counter
        public void ChangeState()
        {
            if (_sequence.ContainsKey(_ticks))
            {
                var newState = _sequence[_ticks].state;
                _sequence[_ticks].types.ForEach(type => states[type] = newState);
            }
            _ticks++;
            _ticks = _ticks % _totalTicks;
            UpdateVisuals();
        }

        public void Display()
        {
            transform.parent = _edge.transform;
            
            var position = _edge.other.originPoint.position.toWorld();
            var right = new Vector3(-_edge.other.originPoint.forward.y, 0, _edge.other.originPoint.forward.x);
            position += right * (LANE_WIDTH * _edge.outgoingLanes.Count
                + LINE_WIDTH * Mathf.Clamp(_edge.outgoingLanes.Count - 1, 0, Mathf.Infinity)
                + MIDDLE_LINE_WIDTH / 2f
                + BORDER_LINE_WIDTH
                - ((_edge.outgoingLanes.Count > 0) ? 0 : MIDDLE_LINE_WIDTH / 2f)
                + TRAFFICLIGHT_OFFSET);
            var rotation = Quaternion.LookRotation(_edge.other.originPoint.forward.toWorld(), Vector3.up);

            var stand = Object.Instantiate(Resources.Load("TrafficLights/TrafficLightStand", typeof(GameObject)) as GameObject, position, rotation);
            stand.name = $"TrafficLightStand ({gameObject.GetInstanceID()})";
            stand.transform.parent = transform;

            var flatLight = Object.Instantiate(Resources.Load("TrafficLights/2DTrafficLight", typeof(GameObject)) as GameObject, position, rotation);
            flatLight.name = $"2DTrafficLight ({gameObject.GetInstanceID()})";
            flatLight.transform.parent = transform;
            flatLight.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/RedOff");
            flatLight.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/YellowOff");
            flatLight.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/GreenOff");

            position += Vector3.up * 3;
            var light = Object.Instantiate(Resources.Load("TrafficLights/TrafficLight", typeof(GameObject)) as GameObject, position, rotation);
            light.name = $"TrafficLight ({gameObject.GetInstanceID()})";
            light.transform.parent = transform;
            light.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/RedOff");
            light.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/YellowOff");
            light.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("TrafficLights/GreenOff");

            UpdateVisuals();
        }
    }
}