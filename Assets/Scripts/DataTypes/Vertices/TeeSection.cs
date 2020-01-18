using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataTypes
{
    public class TeeSection : Vertex
    {
        protected Anchor throughAnchor1;
        protected Anchor throughAnchor2;
        protected Anchor turnAnchor;
        
        public TeeSection(IEnumerable<Anchor> anchors) : base(anchors)
        {
            if (this.anchors.Length != 3)
            {
                throw new WrongAnchorCount("T-sections require exactly 3 anchors!");
            }

            throughAnchor1 = this.anchors[0];
            throughAnchor2 = this.anchors[1];
            turnAnchor = this.anchors[2];
        }

        public TeeSection(Anchor throughAnchor1, Anchor throughAnchor2, Anchor turnAnchor)
            : base(ImmutableArray.Create(throughAnchor1, throughAnchor2, turnAnchor))
        {
            this.throughAnchor1 = throughAnchor1;
            this.throughAnchor2 = throughAnchor2;
            this.turnAnchor = turnAnchor;
        }
    }
}
