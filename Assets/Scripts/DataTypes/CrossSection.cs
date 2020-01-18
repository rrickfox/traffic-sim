using System.Collections.Generic;
namespace DataTypes
{
    public class CrossSection
    {
        public Anchor anchor0;
        public Anchor anchor1;
        public Anchor anchor2;
        public Anchor anchor3;

        public CrossSection(Anchor anchor0, Anchor anchor1, Anchor anchor2, Anchor anchor3)
        {
            this.anchor0 = anchor0;
            this.anchor1 = anchor1;
            this.anchor2 = anchor2;
            this.anchor3 = anchor3;
        }
    }
}