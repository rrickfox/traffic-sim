using System.Collections.Immutable;

namespace DataTypes
{
    public class TeeSection : Vertex
    {
        protected Edge throughOrRight;
        protected Edge throughOrLeft;
        protected Edge leftOrRight;

        public TeeSection(Edge throughOrRight, Edge throughOrLeft, Edge leftOrRight)
            : base(ImmutableArray.Create(throughOrRight, throughOrLeft, leftOrRight))
        {
            this.throughOrRight = throughOrRight;
            this.throughOrLeft = throughOrLeft;
            this.leftOrRight = leftOrRight;
        }
    }
}