namespace DataTypes
{
    public class TeeSection : Vertex
    {
        private Edge throughOrRight { get; }
        private Edge throughOrLeft { get; }
        private Edge leftOrRight { get; }

        public TeeSection(Edge throughOrRight, Edge throughOrLeft, Edge leftOrRight)
            : base(throughOrRight, throughOrLeft, leftOrRight)
        {
            this.throughOrRight = throughOrRight;
            this.throughOrLeft = throughOrLeft;
            this.leftOrRight = leftOrRight;
        }
    }
}