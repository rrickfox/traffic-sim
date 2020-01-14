using UnityEngine;
namespace DataTypes
{
    public class BaseAnchor
    {
        public Vector2 position;
        
        public BaseAnchor(Vector2 position)
        {
            this.position = position;
        }
    }

    public class StartingAnchor : BaseAnchor
    {
        public StartingAnchor(Vector2 position) : base(position) {}
    }
    
    public class EndingAnchor : BaseAnchor
    {
        public EndingAnchor(Vector2 position) : base(position) {}
    }

    public struct CombinedAnchors
    {
        public StartingAnchor start;
        public EndingAnchor end;
    }
}