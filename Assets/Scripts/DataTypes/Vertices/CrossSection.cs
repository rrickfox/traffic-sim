using System.Collections.Immutable;

namespace DataTypes
{
    public class CrossSection : Vertex
    {
        protected Anchor upAnchor;
        protected Anchor rightAnchor;
        protected Anchor downAnchor;
        protected Anchor leftAnchor;

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