using UnityEngine;

namespace DataTypes
{
    public struct Anchor
    {
        public Road parentRoad;
        public Vector2 position;
        public int startingLanes;
        public int endingLanes;
        
        public Anchor(Road parentRoad, Vector2 position, int startingLanes, int endingLanes)
        {
            this.parentRoad = parentRoad;
            this.position = position;
            this.startingLanes = startingLanes;
            this.endingLanes = endingLanes;
        }
    }
}