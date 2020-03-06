namespace DataTypes
{
    public struct RouteSegment
    {
        public ITrack track;
        public LaneType laneType;

        public RouteSegment(ITrack track, LaneType laneType)
        {
            this.track = track;
            this.laneType = laneType;
        }
    }
}