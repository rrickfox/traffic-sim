namespace DataTypes
{
    public class TeeSection : Vertex
    {
        protected Edge throughOrRight;
        protected Edge throughOrLeft;
        protected Edge leftOrRight;

        public TeeSection(Edge throughOrRight, Edge throughOrLeft, Edge leftOrRight)
            : base(throughOrRight, throughOrLeft, leftOrRight)
        {
            this.throughOrRight = throughOrRight;
            this.throughOrLeft = throughOrLeft;
            this.leftOrRight = leftOrRight;
        }
    }
}