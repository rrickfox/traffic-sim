using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using UnityEngine;

namespace DataTypes
{
    public class EndPoint : Vertex
    {
        private Edge _edge;
        private GameObject _carPrefab;
        private GameObject _roadPrefab;
        // ticks before a car spawns on a lane (index)
        private int[] _spawnFrequencies;
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
        
        public void FindPath(IEnumerable<Vertex> vertices, EndPoint end)
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
    
            // creates dictionary for saving path corresponding to two EndPoints
            routingTable.Add(end, DetermineFoundPath(end));
        }

        // iterates over vertices in reverse order to determine path and translates it into a path of edges
        private List<Edge> DetermineFoundPath(Vertex end)
        {
            if (end.pathDistance != null)
            {
                var vertexPath = new LinkedList<Vertex>();
                var edgePath = new List<Edge>();
                while (this != end)
                {
                    vertexPath.AddFirst(end);
                    end = end.previousVertex;
                }
                
                foreach (var vertex in vertexPath)
                {
                    var index = vertexPath.ToList().IndexOf(vertex);
                    edgePath.Add(vertex.GetEdge(vertexPath.ToList()[index+1]));
                }
                return edgePath;
            }
            return null;
        }
        
        public void SpawnCars()
        {
            for(var lane = 0; lane < _edge.outgoingLanes.Count; lane++)
            {
                if(_ticks % _spawnFrequencies[lane] == 0)
                {
                    CreateCar(lane);
                }
            }
            _ticks++;
        }
    
        private void CreateCar(float lane)
        {
            // construct car
            var car = new Car(_edge, 0, lane);
        
            // display the car graphically
            var position = _edge.GetAbsolutePosition(car.positionOnRoad, lane);
            var spawnPoint = new Vector3(position.x, _roadPrefab.transform.localScale.y / 2 + _carPrefab.transform.localScale.y / 2, position.y);
            var rotation = Quaternion.Euler(0, _edge.angle, 0);
            var carGameObject = Object.Instantiate(_carPrefab, spawnPoint, rotation);
            carGameObject.name = "Car_" + CarId.id;
            car.carTransform = carGameObject.transform;
        }

        public void DespawnCars()
        {
            foreach(var car in _edge.cars.ToList()) 
            {
                if (car.positionOnRoad > _edge.length)
                {
                    Object.Destroy(car.carTransform.gameObject);
                    _edge.cars.Remove(car);
                }
            }
        }
    }
}