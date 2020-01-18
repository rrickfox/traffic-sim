using System.Collections.Generic;
namespace DataTypes
{
    public class Roundabout
    {
        public List<Anchor> anchors = new List<Anchor>();

        private List<TeeSection> _teeSections = new List<TeeSection>();
        private List<Road> _roads = new List<Road>();

        public Roundabout(List<Anchor> anchors)
        {
            this.anchors = anchors;
        }
    }
}