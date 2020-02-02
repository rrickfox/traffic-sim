using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using UnityEngine;

namespace DataTypes
{
    public class EndPoint : Vertex
    {
        private Edge _edge { get; }
        private GameObject _carPrefab { get; }
        private GameObject _roadPrefab { get; }
        // ticks before a car spawns on a lane (index)
        private int[] _spawnFrequencies { get; }
        // counter for ticks since start
        private int _ticks = 0;
        public Dictionary<Vertex, List<Edge>> routingTable { get; } = new Dictionary<Vertex, List<Edge>>();
        
        public EndPoint(Edge edge, GameObject carPrefab, GameObject roadPrefab, int[] spawnFrequencies) : base(edge)
        {
            _edge = edge;
            _carPrefab = carPrefab;
            _roadPrefab = roadPrefab;
            _spawnFrequencies = spawnFrequencies;
        }
        
        public void FindPath(ICollection<Vertex> vertices, EndPoint end)
        {
            var tempVertices = vertices.ToHashSet();
            pathDistance = 0;

            // calculates pathDistance and corresponding previousVertex for entire graph
            while (tempVertices.Any(v => v.pathDistance != null))
            {
                // finds vertex with lowest pathDistance, updates its neigbourhood and removes it from tempVertices
                var minVertex = tempVertices.Where(v => v.pathDistance != null).MinBy(v => v.pathDistance).First();
                minVertex.CheckNeigbourhood();
                tempVertices.Remove(minVertex);
            }
            
            // creates dictionary for saving path corresponding to end point
            routingTable.Add(end, DetermineFoundPath(end));
            
            // reset the temporary properties
            foreach (var vertex in vertices)
            {
                vertex.pathDistance = null;
                vertex.previousVertex = null;
            }
        }

        // iterates over vertices in reverse order to determine path and translates it into a path of edges
        private List<Edge> DetermineFoundPath(Vertex end)
        {
            // return null if no path could be found
            if (end.pathDistance == null) return null;
            
            // build the path of all vertices
            var vertexPath = new LinkedList<Vertex>();
            for (var tempEnd = end; tempEnd != this; tempEnd = tempEnd.previousVertex)
            {
                vertexPath.AddFirst(tempEnd);
            }
            
            // return the edges connecting the vertices in the path
            return vertexPath.Zip(vertexPath.Skip(1), (v1, v2) => v1.GetEdge(v2)).ToList();
        }
        
        public void SpawnCars()
        {
            for(var lane = 0; lane < _edge.outgoingLanes.Count; lane++)
            {
                if(_ticks % _spawnFrequencies[lane] == 0)
                {
                    new Car(_carPrefab, _edge, 0, lane);
                }
            }
            _ticks++;
        }

        public void DespawnCars()
        {
            foreach(var car in _edge.cars.ToList()) 
            {
                if (car.positionOnRoad > _edge.length)
                {
                    car.Dispose();
                    _edge.cars.Remove(car);
                }
            }
        }
    }
}