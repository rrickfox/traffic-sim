using System.Collections.Immutable;

namespace DataTypes
{
    public class TeeSection : Vertex
    {
        protected Anchor throughOrRightAnchor;
        protected Anchor throughOrLeftAnchor;
        protected Anchor leftOrRightAnchor;

        public TeeSection(Anchor throughOrRightAnchor, Anchor throughOrLeftAnchor, Anchor leftOrRightAnchor)
            : base(ImmutableArray.Create(throughOrRightAnchor, throughOrLeftAnchor, leftOrRightAnchor))
        {
            this.throughOrRightAnchor = throughOrRightAnchor;
            this.throughOrLeftAnchor = throughOrLeftAnchor;
            this.leftOrRightAnchor = leftOrRightAnchor;
        }
    }
}
