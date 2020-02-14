namespace DataTypes
{
    public interface ITrack
    {
        RoadShape shape {get; set;}
        float length {get;}
        RoadPoint GetAbsolutePosition(float positionOnRoad, float lane);
    }
}