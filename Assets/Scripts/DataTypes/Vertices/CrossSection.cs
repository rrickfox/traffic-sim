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
        private Vector2 center;

        public CrossSection(GameObject prefab, Edge up, Edge right, Edge down, Edge left)
            : base(prefab, up, right, down, left)
        {
            _up = up;
            _right = right;
            _down = down;
            _left = left;
            center = (_up.originPoint.position + _down.originPoint.position + _left.originPoint.position + _right.originPoint.position) / 4f;
            Display();
        }

        public CrossSection(Edge up, Edge right, Edge down, Edge left) : this(EMPTY_PREFAB, up, right, down, left) {}

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

        public void Display()
        {
            // move originPoints to be in line with the borders of neighbouring edges
            _up.UpdateOriginPoint(center // center of section
                + _up.originPoint.forward // move-direction
                * (_right.incomingLanes.Count > 0 && _left.outgoingLanes.Count > 0 ? // check if both edges have lanes on same side
                    (Mathf.Max(_right.incomingLanes.Count, _left.outgoingLanes.Count) * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH 
                    + MIDDLE_LINE_WIDTH / 2f 
                    + BORDER_LINE_WIDTH) // offset to the border of bigger edge
                : BORDER_LINE_WIDTH - MIDDLE_LINE_WIDTH / 2f)); // offset if no lanes are on this side

            _right.UpdateOriginPoint(center
                + _right.originPoint.forward
                * (_down.incomingLanes.Count > 0 && _up.outgoingLanes.Count > 0 ?
                    (Mathf.Max(_down.incomingLanes.Count, _up.outgoingLanes.Count) * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH
                    + MIDDLE_LINE_WIDTH / 2f
                    + BORDER_LINE_WIDTH)
                : BORDER_LINE_WIDTH - MIDDLE_LINE_WIDTH / 2f));

            _down.UpdateOriginPoint(center
                + _down.originPoint.forward
                * (_left.incomingLanes.Count > 0 && _right.outgoingLanes.Count > 0 ?
                    (Mathf.Max(_left.incomingLanes.Count, _right.outgoingLanes.Count) * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH
                    + MIDDLE_LINE_WIDTH / 2f
                    + BORDER_LINE_WIDTH)
                : BORDER_LINE_WIDTH - MIDDLE_LINE_WIDTH / 2f));

            _left.UpdateOriginPoint(center
                + _left.originPoint.forward
                * (_up.incomingLanes.Count > 0 && _down.outgoingLanes.Count > 0 ?
                    (Mathf.Max(_up.incomingLanes.Count, _down.outgoingLanes.Count) * (LANE_WIDTH + LINE_WIDTH)
                    - LINE_WIDTH
                    + MIDDLE_LINE_WIDTH / 2f
                    + BORDER_LINE_WIDTH)
                : BORDER_LINE_WIDTH - MIDDLE_LINE_WIDTH / 2f));
        }
    }
    
    public class CrossSectionBehaviour : VertexBehaviour<CrossSection> { }
}