using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    public class TeeSection : Vertex<TeeSection, TeeSectionBehaviour>
    {
        private Edge _throughOrRight { get; }
        private Edge _throughOrLeft { get; }
        private Edge _leftOrRight { get; }

        public TeeSection(GameObject prefab, Edge throughOrRight, Edge throughOrLeft, Edge leftOrRight)
            : base(prefab, throughOrRight, throughOrLeft, leftOrRight)
        {
            _throughOrRight = throughOrRight;
            _throughOrLeft = throughOrLeft;
            _leftOrRight = leftOrRight;
        }

        // returns necessary lane to go from an edge to another edge
        // throws exception if edges are not in this vertex
        // throws exception if edges are equal
        protected LaneType SubRoute(Edge from, Edge to)
        {
            if(this.edges.Contains(from) && this.edges.Contains(to))
                if(this._leftOrRight.Equals(from))
                    if(this._leftOrRight.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(this._throughOrLeft.Equals(to))
                        return LaneType.RightTurn;
                    else
                        return LaneType.LeftTurn;
                else if(this._throughOrLeft.Equals(from))
                    if(this._throughOrLeft.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(this._throughOrRight.Equals(to))
                        return LaneType.Through;
                    else
                        return LaneType.LeftTurn;
                else // from lane is through or right lane
                    if(this._throughOrRight.Equals(to))
                        throw new System.Exception("From and to are the same Edge");
                    else if(this._leftOrRight.Equals(to))
                        return LaneType.RightTurn;
                    else
                        return LaneType.Through;
            else
                throw new System.Exception("Edges not found");
        }
    }

    public class TeeSectionBehaviour : VertexBehaviour<TeeSection> { }
}