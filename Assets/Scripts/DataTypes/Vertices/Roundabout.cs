using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    public class Roundabout : Vertex<Roundabout, RoundaboutBehaviour>
    {
        private Roundabout(GameObject prefab, IEnumerable<Edge> edges) : base(prefab, edges)
        {
        }
    }

    public class RoundaboutBehaviour : VertexBehaviour<Roundabout> { }
}