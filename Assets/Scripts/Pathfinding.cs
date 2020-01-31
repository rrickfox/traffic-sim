using System.Collections.Generic;
using System.Linq;
using DataTypes;
using MoreLinq;

public static class Pathfinding
{
    public static void StartPathfinding(List<Vertex> vertices)
    {
        foreach (var start in vertices.OfType<EndPoint>())
        {
            foreach (var end in vertices.OfType<EndPoint>().Where(end => end != start))
            {
                CalculateVertexParameters(vertices, start, end);
            }
        }
    }
    
    private static void CalculateVertexParameters(List<Vertex> vertices, EndPoint start, EndPoint end)
    {
        var tempVertices = vertices;
        start.pathDistance = 0;
        Vertex minVertex;
    
        // calculates pathDistance and corresponding previousVertex for entire graph
        while (tempVertices.Count != 0)
        {
            // finds vertex with lowest pathDistance, updates its neigbourhood and removes it from tempVertices
            minVertex = tempVertices.MinBy(v => v.pathDistance).First();
            minVertex.CheckNeigbourhood();
            tempVertices.Remove(minVertex);
        }
    
        // creates dictionary for saving path corresponding to two EndPoints
        start.routingTable.Add(end, DeterminePath(start, end));
    }

    // recursively iterates over vertices in reverse order to determine path
    private static List<Vertex> DeterminePath(EndPoint start, Vertex end)
    {
        var path = new List<Vertex>();
        while (start != end)
        {
            path.Add(end);
            end = end.previousVertex;
        }
        path.Reverse();
        return path;
    }
}