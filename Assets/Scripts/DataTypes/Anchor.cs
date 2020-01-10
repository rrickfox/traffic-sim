namespace DataTypes
{
    public class BaseAnchor
    {
        
    }

    public class StartingAnchor : BaseAnchor
    {
        
    }
    
    public class EndingAnchor : BaseAnchor
    {
        
    }

    public struct CombinedAnchors
    {
        public StartingAnchor Start;
        public EndingAnchor End;
    }
}