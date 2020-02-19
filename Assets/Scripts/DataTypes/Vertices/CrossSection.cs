using System;
using UnityEngine;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public class CrossSection : Vertex<CrossSection, CrossSectionBehaviour>
    {
        private Edge _up { get; }
        private Edge _right { get; }
        private Edge _down { get; }
        private Edge _left { get; }

        public CrossSection(GameObject prefab, Edge up, Edge right, Edge down, Edge left)
            : base(prefab, up, right, down, left)
        {
            _up = up;
            _right = right;
            _down = down;
            _left = left;
        }

        public CrossSection(Edge up, Edge right, Edge down, Edge left) : this(EMPTY_PREFAB, up, right, down, left) {}

        // returns necessary lane to go from an edge to another edge
        // throws exception if edges are not in this vertex
        // throws exception if edges are equal
        public override LaneType SubRoute(Edge comingFrom, Edge to)
        {
            var from = comingFrom.other; // Subroute gets called with the Edge facing this Vertex, therefore other must be called
            if (!edges.Contains(from)) throw new Exception("From Edge not found");
            if(!edges.Contains(to)) throw new Exception("To Edge not found");
            if(from == to) throw new Exception("From and to are the same Edge");
            
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
    
    public class CrossSectionBehaviour : VertexBehaviour<CrossSection> { }
}