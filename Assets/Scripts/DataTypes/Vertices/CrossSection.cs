using System.Collections.Immutable;

namespace DataTypes
{
    public class CrossSection : Vertex
    {
        protected Edge up;
        protected Edge right;
        protected Edge down;
        protected Edge left;

        public CrossSection(Edge up, Edge right, Edge down, Edge left)
            : base(ImmutableArray.Create(up, right, down, left))
        {
            this.up = up;
            this.right = right;
            this.down = down;
            this.left = left;
        }
    }
}