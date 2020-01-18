using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataTypes
{
    public class CrossSection : Vertex
    {
        protected Anchor upAnchor;
        protected Anchor rightAnchor;
        protected Anchor downAnchor;
        protected Anchor leftAnchor;
        
        public CrossSection(IEnumerable<Anchor> anchors) : base(anchors)
        {
            if (this.anchors.Length != 4)
            {
                throw new WrongAnchorCount("Cross sections require exactly 4 anchors!");
            }

            this.upAnchor = this.anchors[0];
            this.rightAnchor = this.anchors[1];
            this.downAnchor = this.anchors[2];
            this.leftAnchor = this.anchors[3];
        }

        public CrossSection(Anchor upAnchor, Anchor rightAnchor, Anchor downAnchor, Anchor leftAnchor)
            : base(ImmutableArray.Create(upAnchor, rightAnchor, downAnchor, leftAnchor))
        {
            this.upAnchor = upAnchor;
            this.rightAnchor = rightAnchor;
            this.downAnchor = downAnchor;
            this.leftAnchor = leftAnchor;
        }
    }
}