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
        public override LaneType SubRoute(Edge from, Edge to)
        {
            if(edges.Contains(from) && this.edges.Contains(to))
                if(_up.Equals(from))
                    if(_up.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(_right.Equals(to))
                        return LaneType.LeftTurn;
                    else if(_down.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
                else if(_right.Equals(from))
                    if(_right.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(_down.Equals(to))
                        return LaneType.LeftTurn;
                    else if(_left.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
                else if(_down.Equals(from))
                    if(_down.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(_left.Equals(to))
                        return LaneType.LeftTurn;
                    else if(_up.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
                else
                    if(_left.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(_up.Equals(to))
                        return LaneType.LeftTurn;
                    else if(_right.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
            else
                throw new System.Exception("Edges not found");
        }
    }
    
    public class CrossSectionBehaviour : VertexBehaviour<CrossSection> { }
}