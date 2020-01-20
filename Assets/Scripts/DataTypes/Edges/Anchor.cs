using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataTypes
{
    public class Anchor : Point
    {
        public Road parentRoad;
        public AnchorNumber number;
        public ImmutableArray<Lane> endingLanes;

        public Anchor(Point point, Road parentRoad, AnchorNumber number, IEnumerable<Lane> endingLanes) : base(point)
        {
            this.parentRoad = parentRoad;
            this.number = number;
            this.endingLanes = endingLanes.ToImmutableArray();
        }
    }
}