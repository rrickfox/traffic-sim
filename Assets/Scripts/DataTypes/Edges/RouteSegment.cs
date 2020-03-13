namespace DataTypes
{
    public struct RouteSegment
    {
        public Edge edge;
        public LaneType laneType;

        public RouteSegment(Edge edge, LaneType laneType)
        {
            this.edge = edge;
            this.laneType = laneType;
        }
    }
}