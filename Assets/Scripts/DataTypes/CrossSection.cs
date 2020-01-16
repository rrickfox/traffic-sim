using System.Collections.Generic;
namespace DataTypes
{
    public class CrossSection
    {
        public Node node0;
        public Node node1;
        public Node node2;
        public Node node3;

        public CrossSection(Node node0, Node node1, Node node2, Node node3)
        {
            this.node0 = node0;
            this.node1 = node1;
            this.node2 = node2;
            this.node3 = node3;
        }
    }
}