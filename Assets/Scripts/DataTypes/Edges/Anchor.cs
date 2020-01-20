using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace DataTypes
{
    public struct Anchor
    {
        public Road parentRoad;
        public AnchorNumber number;
        public Vector2 position;
        public ImmutableArray<Lane> endingLanes;

        public Anchor(AnchorNumber number, Vector2 position, IEnumerable<Lane> endingLanes)
        {
            parentRoad = null;
            this.number = number;
            this.position = position;
            this.endingLanes = endingLanes.ToImmutableArray();
        }
    }
}