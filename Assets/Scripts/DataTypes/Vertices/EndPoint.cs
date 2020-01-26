using System.Collections.Immutable;
using UnityEngine;

namespace DataTypes
{
    class EndPoint : Vertex
    {
        protected Anchor anchor;
        public CarSpawner spawner;

        public EndPoint(Anchor anchor, GameObject carPrefab, GameObject roadPrefab)
            : base(ImmutableArray.Create(anchor))
        {
            this.anchor = anchor;
            spawner = new CarSpawner(carPrefab, roadPrefab);
        }
    }
}