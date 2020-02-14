namespace DataTypes
{
    public interface ITrack
    {
        RoadShape shape {get;}
        float length {get;}
        RoadPoint GetAbsolutePosition(float positionOnRoad, float lane);
    }
}