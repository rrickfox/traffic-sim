namespace DataTypes
{
    public class Edge : RoadView
    {
        // allows this derived class to treat _other as an instance of Edge and not just RoadView
        public new Edge other { get => (Edge)_other; set => _other = value; }
        // the Vertex from which this edge originates
        public Vertex vertex;

        public Edge(RoadView view, Vertex vertex, RoadView otherView, Vertex otherVertex) : base(view)
        {
            other = new Edge(otherView, otherVertex, this);
            this.vertex = vertex;
        }

        private Edge(RoadView view, Vertex vertex, Edge otherEdge) : base(view)
        {
            other = otherEdge;
            this.vertex = vertex;
        }
    }
}