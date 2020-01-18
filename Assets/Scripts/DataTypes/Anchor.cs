using UnityEngine;

namespace DataTypes
{
    public struct Anchor
    {
        public Vector2 position;
        public int startingLanes;
        public int endingLanes;
        
        public Anchor(Vector2 position, int startingLanes, int endingLanes)
        {
            this.position = position;
            this.startingLanes = startingLanes;
            this.endingLanes = endingLanes;
        }
    }
}