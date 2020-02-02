namespace DataTypes
{
    public class TeeSection : Vertex<TeeSection, TeeSectionBehaviour>
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
            
            CreateGameObject();
        }
    }

    public class TeeSectionBehaviour : VertexBehaviour<TeeSection> { }
}