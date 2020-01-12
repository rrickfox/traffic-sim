using UnityEngine;
namespace DataTypes
{
    public class BaseAnchor
    {
        
    }

    public class StartingAnchor : BaseAnchor
    {
    	public Vector2 position;
        
        public StartingAnchor(Vector2 setPosition)
        {
            position = setPosition;
        }
    }
    
    public class EndingAnchor : BaseAnchor
    {
        public Vector2 position;
        
        public EndingAnchor(Vector2 setPosition)
        {
            position = setPosition;
        }
    }

    public struct CombinedAnchors
    {
        public StartingAnchor start;
        public EndingAnchor end;
    }
}