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

            upAnchor = this.anchors[0];
            rightAnchor = this.anchors[1];
            downAnchor = this.anchors[2];
            leftAnchor = this.anchors[3];
        }

        public CrossSection(Anchor upAnchor, Anchor rightAnchor, Anchor downAnchor, Anchor leftAnchor)
            : this(ImmutableArray.Create(upAnchor, rightAnchor, downAnchor, leftAnchor)) {}
    }
}