using UnityEngine;
using System;
namespace DataTypes
{
    public class Road
    {
        public readonly int id;
        public CombinedAnchors anchors1;
        public CombinedAnchors anchors2;
        public int lanes1To2;
        public int lanes2To1;
        public int length;
        public CurvatureDirection curvatureDirection;

        private CONSTANTS _constants = new CONSTANTS();

        public Road(int id, Vector2 position1, Vector2 position2, int lanes1To2, int lanes2To1)
        {
            this.id = id;
            anchors1 = new CombinedAnchors(position1);
            anchors2 = new CombinedAnchors(position2);
            this.lanes1To2 = lanes1To2;
            this.lanes2To1 = lanes2To1;
            length = (int)Math.Round(Vector2.Distance(position1, position2) / _constants.DISTANCE_UNIT);
        }
    }
}