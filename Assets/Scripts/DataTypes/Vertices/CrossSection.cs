namespace DataTypes
{
    public class CrossSection : Vertex
    {
        private Edge up { get; }
        private Edge right { get; }
        private Edge down { get; }
        private Edge left { get; }

        public CrossSection(Edge up, Edge right, Edge down, Edge left)
            : base(up, right, down, left)
        {
            this.up = up;
            this.right = right;
            this.down = down;
            this.left = left;
        }
    }
}