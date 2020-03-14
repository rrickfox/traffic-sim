using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DataTypes;
using MoreLinq.Extensions;
using UnitsNet;
using Utility;

namespace Pathfinding
{
    public static class Pathfinding
    {
        public static void StartPathfinding(ICollection<Vertex> vertices)
        {
            var verticesSet = vertices.ToHashSet();
            var endPoints = vertices.OfType<EndPoint>().ToList();

            foreach (var start in endPoints)
                foreach (var end in endPoints.Where(end => end != start))
                    start.FindPath(end, verticesSet);
        }
    }

    public static class VertexExtensions
    {
        private static ConditionalWeakTable<Vertex, VertexExtensionsData> _DATA { get; } =
            new ConditionalWeakTable<Vertex, VertexExtensionsData>();

        // distance value relative to start point of pathfinding
        private static Length? GetPathDistance(this Vertex self)
            => _DATA.GetOrCreateValue(self).pathDistance;
        private static void SetPathDistance(this Vertex self, Length? value)
            => _DATA.GetOrCreateValue(self).pathDistance = value;
        
        // current candidate for predecessor in path
        private static Vertex GetPreviousVertex(this Vertex self)
            => _DATA.GetOrCreateValue(self).previousVertex;
        private static void SetPreviousVertex(this Vertex self, Vertex value)
            => _DATA.GetOrCreateValue(self).previousVertex = value;

        private static Edge GetEdge(this Vertex self, Vertex neighbour)
        {
            return self.edges.FirstOrDefault(edge => edge.other.vertex == neighbour);
        }
        
        // checks neighbourhood for necessary updates in pathfinding attributes
        private static void CheckNeighbourhood(this Vertex self)
        {
            var pathDistance = self.GetPathDistance();
            foreach (var edge in self.edges.Where(edge => edge.outgoingLanes.Count > 0))
            {
                var otherPathDistance = edge.other.vertex.GetPathDistance();
                var newPathDistance = pathDistance + edge.length;
                if (otherPathDistance != null && otherPathDistance <= newPathDistance) continue;
                edge.other.vertex.SetPathDistance(newPathDistance);
                edge.other.vertex.SetPreviousVertex(self);
            }
        }

        public static void FindPath(this EndPoint self, EndPoint end, ICollection<Vertex> vertices)
        {
            var tempVertices = vertices.ToHashSet();
            self.SetPathDistance(Length.Zero);

            // calculates pathDistance and corresponding previousVertex for entire graph
            while (tempVertices.Any(v => v.GetPathDistance() != null))
            {
                // finds vertex with lowest pathDistance, updates its neighbourhood and removes it from tempVertices
                var minVertex = tempVertices.Where(v => v.GetPathDistance() != null).MinBy(v => v.GetPathDistance()).First();
                minVertex.CheckNeighbourhood();
                tempVertices.Remove(minVertex);
            }

            // creates dictionary for saving path corresponding to end point
            self.routingTable.Add(end, self.DetermineFoundPath(end));

            // reset the temporary properties
            foreach (var vertex in vertices)
            {
                vertex.SetPathDistance(null);
                vertex.SetPreviousVertex(null);
            }
        }
        
        // iterates over vertices in reverse order to determine path and translates it into a path of edges
        private static List<RouteSegment> DetermineFoundPath(this EndPoint self, Vertex end)
        {
            // return null if no path could be found
            if (end.GetPathDistance() == null)
                return null;

            // build the path of all vertices
            var vertexPath = new LinkedList<Vertex>();
            for (var tempEnd = end; tempEnd != self; tempEnd = tempEnd.GetPreviousVertex())
                vertexPath.AddFirst(tempEnd);
            vertexPath.AddFirst(self);

            // return the route segments composed of edges connecting the vertices
            // as well as the LaneType required at the vertex
            var path = vertexPath.ZipThree(
                vertexPath.Skip(1),
                vertexPath.Skip(2),
                (v1, v2, v3) =>
                    new RouteSegment(edge: v1.GetEdge(v2), laneType: v2.SubRoute(v1.GetEdge(v2), v2.GetEdge(v3)))
            ).ToList();
            path.Add(new RouteSegment(
                edge: vertexPath.Last.Previous.Value.GetEdge(vertexPath.Last.Value),
                laneType: LaneType.Through // since last vertex is an EndPoint, LaneType must be Through
            ));
            return path;
        }
    }
    
    public class VertexExtensionsData
    {
        public Length? pathDistance; // distance value relative to start point of pathfinding
        public Vertex previousVertex; // current candidate for predecessor in path
    }
}