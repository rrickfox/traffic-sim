using System;
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
        public override LaneType SubRoute(Edge from, Edge to)
        {
            if (!edges.Contains(from)) throw new Exception("From Edge not found");
            if(!edges.Contains(to)) throw new Exception("To Edge not found");
            if(from == to) throw new Exception("From and to are the same Edge");
            
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

    public class TeeSectionBehaviour : VertexBehaviour<TeeSection> { }
}