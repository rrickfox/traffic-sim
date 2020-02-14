using UnityEngine;

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

        // returns necessary lane to go from an edge to another edge
        // throws exception if edges are not in this vertex
        // throws exception if edges are equal
        public override LaneType SubRoute(Edge from, Edge to)
        {
            if(this.edges.Contains(from) && this.edges.Contains(to))
                if(this._up.Equals(from))
                    if(this._up.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(this._right.Equals(to))
                        return LaneType.LeftTurn;
                    else if(this._down.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
                else if(this._right.Equals(from))
                    if(this._right.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(this._down.Equals(to))
                        return LaneType.LeftTurn;
                    else if(this._left.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
                else if(this._down.Equals(from))
                    if(this._down.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(this._left.Equals(to))
                        return LaneType.LeftTurn;
                    else if(this._up.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
                else
                    if(this._left.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(this._up.Equals(to))
                        return LaneType.LeftTurn;
                    else if(this._right.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.RightTurn;
            else
                throw new System.Exception("Edges not found");
        }
    }
    
    public class CrossSectionBehaviour : VertexBehaviour<CrossSection> { }
}