using System.Collections.Generic;
namespace DataTypes
{
    public class Roundabout
    {
        public List<Node> node = new List<Node>();

        private List<TeeSection> _teeSections = new List<TeeSection>();
        private List<Road> _roads = new List<Road>();

        public Roundabout(List<Node> node)
        {
            this.node = node;
        }
    }
}