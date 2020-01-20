using System.Collections.Generic;

namespace DataTypes
{
    public enum LaneType
    {
        LeftTurn,
        Through,
        RightTurn
    }

    public class Lane
    {
        public HashSet<LaneType> types;

        public Lane(HashSet<LaneType> types)
        {
            this.types = types;
        }
    }
}