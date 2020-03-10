using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using Utility;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public class RoadShape
    {
        private List<BezierCurve> _curves;
        public RoadPoint[] points;
        public Length length = Length.Zero;

        // create RoadShape from list of curves
        public RoadShape(List<BezierCurve> curves)
        {
            _curves = curves;

            CalculateEvenlySpacedPoints();
        }

        // create a RoadShape from existing parameters
        private RoadShape(List<BezierCurve> curves, RoadPoint[] points, Length length)
        {
            _curves = curves;
            this.points = points;
            this.length = length;
        }

        // calculates points in regular intervals
        private void CalculateEvenlySpacedPoints()
        {
            // first point not included in any list of points to avoid duplication
            var tempPoints = new List<Vector2> {_curves[0].startPoint};
            
            // populate list with all calculated points of all bezier curves
            foreach (var curve in _curves)
            {
                tempPoints.AddRange(curve.CalculatePoints());
            }

            // evenly spaced will have a distance of DISTANCE_UNIT
            var evenlySpacedPoints = new List<RoadPoint>
            {
                new RoadPoint(tempPoints[0], (tempPoints[1] - tempPoints[0]).normalized)
            };
            // save lastPoint to interpolate between calculated points with distance greater than DISTANCE_UNIT
            var lastPoint = tempPoints[0];
            float dstSinceLastEvenPoint = 0;

            foreach(var point in tempPoints)
            {
                // go along the curve to find new point at regular interval
                dstSinceLastEvenPoint += Vector2.Distance(lastPoint, point);
                
                // checks for overshoot
                while(dstSinceLastEvenPoint >= DISTANCE_UNIT)
                {
                    dstSinceLastEvenPoint -= DISTANCE_UNIT;

                    // goes from lastPoint to this point, with DISTANCE_UNIT length
                    var newEvenlySpacedPoint = lastPoint + (point - lastPoint).normalized * DISTANCE_UNIT;
                    var newRoadPoint = new RoadPoint(newEvenlySpacedPoint, (point - lastPoint).normalized);
                    evenlySpacedPoints.Add(newRoadPoint);

                    // length in DISTANCE_UNITS
                    length += Formulas.FromUnityDistanceUnits(1);
                    
                    lastPoint = newEvenlySpacedPoint;
                }

                if(dstSinceLastEvenPoint == 0)
                {
                    lastPoint = point;
                }
            }

            _curves.Last().endPoint = lastPoint;

            points = evenlySpacedPoints.ToArray();
        }

        // reverse the RoadShape
        public RoadShape Inverse()
        {
            var reverseCurves = _curves.Select(curve => curve.Revert()).Reverse().ToList();
            var reversePoints = points.Select(point => point.Invert()).Reverse().ToArray();

            return new RoadShape(reverseCurves, reversePoints, length);
        }
    }
}