using UnityEngine;
using Utility;

namespace DataTypes
{
    public class CrossSection : Vertex
    {
        public override GameObject prefab { get; } = CONSTANTS.EMPTY_PREFAB;

        private Edge _up { get; }
        private Edge _right { get; }
        private Edge _down { get; }
        private Edge _left { get; }
        
        public CrossSection(Edge up, Edge right, Edge down, Edge left, int red, int yellow, int green)
            : base(up, right, down, left)
        {
            _up = up;
            _up.other.light = new TrafficLight(red, yellow, green, this);
            _right = right;
            // calculates cycles based on perpendicular street
            _right.other.light = new TrafficLight(yellow + green, yellow, red - yellow, this);
            _down = down;
            _down.other.light = _up.other.light;
            _left = left;
            _left.other.light = _right.other.light;
        }

        // returns necessary lane to go from an edge to another edge
        // throws exception if edges are not in this vertex
        // throws exception if edges are equal
        public override LaneType SubRoute(Edge comingFrom, Edge to)
        {
            var from = comingFrom.other; // Subroute gets called with the Edge facing this Vertex, therefore other must be called
            if (!edges.Contains(from)) throw new NetworkConfigurationError("From Edge not found");
            if(!edges.Contains(to)) throw new NetworkConfigurationError("To Edge not found");
            if(from == to) throw new NetworkConfigurationError("From and to are the same Edge");
            
            if(from == _up)
                if(to == _right)
                    return LaneType.LeftTurn;
                else if(to == _down)
                    return LaneType.Through;
                else // to == _left
                    return LaneType.RightTurn;
            if(from == _right)
                if(to == _down)
                    return LaneType.LeftTurn;
                else if(to == _left)
                    return LaneType.Through;
                else // to == _up
                    return LaneType.RightTurn;
            if(from == _down)
                if(to == _left)
                    return LaneType.LeftTurn;
                else if(to == _up)
                    return LaneType.Through;
                else // to == _right
                    return LaneType.RightTurn;
            else // from == _left
                if(to == _up)
                    return LaneType.LeftTurn;
                else if(to == _right)
                    return LaneType.Through;
                else // to == _down
                    return LaneType.RightTurn;
        }
    }
}