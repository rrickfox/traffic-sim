using System.Collections.Generic;
using System.Linq;
using DataTypes;
using MoreLinq.Extensions;

public class Pathfinding
{
    public static void StartPathfinding(ICollection<IVertex> vertices)
    {
        var verticesSet = vertices.ToHashSet();
        var endPoints = vertices.OfType<EndPoint>().ToList();
        foreach (var start in endPoints)
        {
            foreach (var end in endPoints.Where(end => end != start))
            {
                start.FindPath(verticesSet, end);
            }
        }
    }
}