using DataTypes;

namespace Data
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