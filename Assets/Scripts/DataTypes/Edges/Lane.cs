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
        public HashSet<LaneType> types { get; private set;}

        public Lane(HashSet<LaneType> types)
        {
            this.types = types;
        }

        // set lanetypes to through, needed for subRoute
        public void ResetLaneTypes(LaneType type)
        {
            types = new HashSet<LaneType>(){type};
        }
    }
}