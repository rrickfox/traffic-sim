namespace DataTypes
{
    public class TeeSection : Vertex
    {
        private Edge _throughOrRight { get; }
        private Edge _throughOrLeft { get; }
        private Edge _leftOrRight { get; }

        public TeeSection(Edge throughOrRight, Edge throughOrLeft, Edge leftOrRight)
            : base(throughOrRight, throughOrLeft, leftOrRight)
        {
            _throughOrRight = throughOrRight;
            _throughOrLeft = throughOrLeft;
            _leftOrRight = leftOrRight;
        }
    }
}