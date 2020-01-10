using DataTypes;

namespace DataTypes
{
    public class Road
    {
        public StartingAnchor startingAnchor;
        public EndingAnchor endingAnchor;
        public uint lanes;
        public uint length;
        public CurvatureDirection curvatureDirection;
    }
}