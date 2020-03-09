using Utility;
using UnityEngine;

namespace DataTypes
{
    public class TeeSection : Vertex
    {
        private Edge _throughOrRight { get; }
        private Edge _throughOrLeft { get; }
        private Edge _leftOrRight { get; }

        public TeeSection(GameObject prefab, Edge throughOrRight, Edge throughOrLeft, Edge leftOrRight, int red, int yellow, int green)
            : base(prefab, throughOrRight, throughOrLeft, leftOrRight)
        {
            _throughOrRight = throughOrRight;
            _throughOrRight.light = new TrafficLight(CONSTANTS.EMPTY_PREFAB, red, yellow, green, this);
            _throughOrLeft = throughOrLeft;
            _throughOrLeft.light = new TrafficLight(CONSTANTS.EMPTY_PREFAB, red, yellow, green, this);
            _leftOrRight = leftOrRight;
            // calculates cycles based on perpendicular street
            _leftOrRight.light = new TrafficLight(CONSTANTS.EMPTY_PREFAB, yellow + green, yellow, red - yellow, this);
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
            
            if(from == _leftOrRight)
                if(to == _throughOrRight)
                    return LaneType.LeftTurn;
                else // to == _throughOrLeft
                    return LaneType.RightTurn;
            if(from == _throughOrRight)
                if(to == _leftOrRight)
                    return LaneType.RightTurn;
                else // to == _throughOrLeft
                    return LaneType.Through;
            else // from == _throughOrLeft
                if(to == _leftOrRight)
                    return LaneType.LeftTurn;
                else // to == _throughOrRight
                    return LaneType.Through;
        }
    }
}