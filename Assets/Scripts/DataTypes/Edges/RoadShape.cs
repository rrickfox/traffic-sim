using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace DataTypes
{
    public class RoadShape
    {
        private List<BezierCurve> _curves;
        public RoadPoint[] points;
        public RoadPoint endingPoint;
        public float length = 0;

        // create RoadShape from list of curves
        public RoadShape(List<BezierCurve> curves)
        {
            _curves = curves;

            CalculateEvenlySpacedPoints();
        }

        // create a RoadShape from existing parameters
        private RoadShape(List<BezierCurve> curves, RoadPoint[] points, RoadPoint endingPoint, float length)
        {
            _curves = curves;
            this.points = points;
            this.endingPoint = endingPoint;
            this.length = length;
        }

        // calculates points in regular intervals
        private void CalculateEvenlySpacedPoints()
        {
            var tempPoints = new List<Vector2> {_curves[0].startPoint};
            
            foreach (var curve in _curves)
            {
                tempPoints.AddRange(curve.CalculatePoints());
            }

            var evenlySpacedPoints = new List<RoadPoint>
            {
                new RoadPoint(tempPoints[0], (tempPoints[1] - tempPoints[0]).normalized)
            };
            var lastPoint = tempPoints[0];
            var secondLastPoint = tempPoints[0];
            float dstSinceLastEvenPoint = 0;

            foreach(var point in tempPoints)
            {
                dstSinceLastEvenPoint += Vector2.Distance(lastPoint, point);
                
                while(dstSinceLastEvenPoint >= CONSTANTS.DISTANCE_UNIT)
                {
                    dstSinceLastEvenPoint -= CONSTANTS.DISTANCE_UNIT;
                    var newEvenlySpacedPoint = lastPoint + (point - lastPoint).normalized * CONSTANTS.DISTANCE_UNIT;
                    var newRoadPoint = new RoadPoint(newEvenlySpacedPoint, (point - lastPoint).normalized);
                    evenlySpacedPoints.Add(newRoadPoint);

                    length++;
                    
                    secondLastPoint = lastPoint;
                    lastPoint = newEvenlySpacedPoint;
                }

                if(dstSinceLastEvenPoint == 0)
                {
                    secondLastPoint = lastPoint;
                    lastPoint = point;
                }
            }

            length += dstSinceLastEvenPoint / CONSTANTS.DISTANCE_UNIT;
            endingPoint = new RoadPoint(lastPoint, (secondLastPoint - lastPoint).normalized);

            points = evenlySpacedPoints.ToArray();
        }

        public RoadShape Inverse()
        {
            var reverseCurves = _curves.Select(curve => curve.Revert()).Reverse().ToList();
            var reversePoints = points.Select(point => point.Invert()).Reverse().ToArray();

            return new RoadShape(reverseCurves, reversePoints, reversePoints.Last(), length);
        }
    }
}