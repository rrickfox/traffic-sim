using System.Collections.Generic;
using System.Linq;
using DataTypes;

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
            Vertex minVertex = start;
        
            // calculates pathDistance and corresponding previousVertex for entire graph
            while (tempVertices.Count != 0)
            {
                // finds vertex with lowest pathDistance, updates its neigbourhood and removes it from tempVertices
                foreach (var vertex in tempVertices.Where(vertex => vertex.pathDistance < minVertex.pathDistance))
                {
                    minVertex = vertex;
                }
                minVertex.CheckNeigbourhood();
                tempVertices.Remove(minVertex);
            }
        
            // creates dictionary for saving path corresponding to two EndPoints
            var path = new List<Vertex>();
            start.routingTable.Add(end, DeterminePath(path, start, end));
        }
    
        // recursively iterates over vertices in reverse order to determine path
        private static List<Vertex> DeterminePath(List<Vertex> path, EndPoint start, Vertex tempEnd)
        {
            if (start != tempEnd)
            {
                path.Append(tempEnd);
                DeterminePath(path, start, tempEnd.previousVertex);
            }

            path.Reverse();
            return path;
        }
    }