using UnityEngine;
using System;
namespace DataTypes
{
    public class Road
    {
        public readonly int id;
        public StartingAnchor startingAnchor;
        public EndingAnchor endingAnchor;
        public int lanesStartToEnd;
        public int lanesEndToStart;
        public int length;
        public CurvatureDirection curvatureDirection;

        private CONSTANTS _constants = new CONSTANTS();

        Road(int setId, Vector2 startPosition, Vector2 endingPosition, int setLanesStartToEnd, int setLanesEndToStart)
        {
            id = setId;
            startingAnchor = new StartingAnchor(startPosition);
            endingAnchor = new EndingAnchor(endingPosition);
            lanesStartToEnd = setLanesStartToEnd;
            lanesEndToStart = setLanesEndToStart;
            length = (int)Math.Round(Vector2.Distance(startPosition, endingPosition) / _constants.DISTANCE_UNIT);
        }
    }
}