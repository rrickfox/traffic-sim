using UnityEngine;

namespace DataTypes
{
    public class CrossSection : VisualVertex<CrossSection, CrossSectionBehaviour>
    {
        private Edge _up { get; }
        private Edge _right { get; }
        private Edge _down { get; }
        private Edge _left { get; }

        public CrossSection(GameObject prefab, Edge up, Edge right, Edge down, Edge left)
            : base(prefab, up, right, down, left)
        {
            _up = up;
            _right = right;
            _down = down;
            _left = left;
        }
    }
    
    public class CrossSectionBehaviour : VertexBehaviour<CrossSection> { }
}