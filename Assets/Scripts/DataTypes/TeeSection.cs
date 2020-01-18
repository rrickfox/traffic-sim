using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataTypes
{
    public class TeeSection : Vertex
    {
        protected Anchor straightAnchor1;
        protected Anchor straightAnchor2;
        protected Anchor endingAnchor;
        
        public TeeSection(IEnumerable<Anchor> anchors) : base(anchors)
        {
            if (this.anchors.Length != 3)
            {
                throw new WrongAnchorCount("T-sections require exactly 3 anchors!");
            }

            straightAnchor1 = this.anchors[0];
            straightAnchor2 = this.anchors[1];
            endingAnchor = this.anchors[2];
        }

        private TeeSection(Anchor straightAnchor1, Anchor straightAnchor2, Anchor endingAnchor)
            : base(ImmutableArray.Create(straightAnchor1, straightAnchor2, endingAnchor))
        {
            this.straightAnchor1 = straightAnchor1;
            this.straightAnchor2 = straightAnchor2;
            this.endingAnchor = endingAnchor;
        }
    }
}