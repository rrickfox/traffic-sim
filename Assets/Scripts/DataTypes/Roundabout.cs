

namespace DataTypes
{
    public class Roundabout
    {
        public CombinedAnchors North;
        public CombinedAnchors East;
        public CombinedAnchors South;
        public CombinedAnchors West;

        private TeeSection _northSection;
        private Road _neRoad;
        private TeeSection _eastSection;
        private Road _esRoad;
        private TeeSection _southSection;
        private Road _swSection;
        private TeeSection _westSection;
        private Road _wnSection;
    }
}