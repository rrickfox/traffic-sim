namespace DataTypes
{
    public class CrossSection : Vertex
    {
        private Edge _up { get; }
        private Edge _right { get; }
        private Edge _down { get; }
        private Edge _left { get; }

        public CrossSection(Edge up, Edge right, Edge down, Edge left)
            : base(up, right, down, left)
        {
            _up = up;
            _right = right;
            _down = down;
            _left = left;
        }
    }
}