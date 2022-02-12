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

        // change start position of first curve
        public void UpdateOrigin(Vector2 newOrigin)
        {
            _curves.First().startPoint = newOrigin;
            CalculateEvenlySpacedPoints();
        }

        // calculates points in regular intervals
        private void CalculateEvenlySpacedPoints()
        {
            length = Length.Zero;
            // first point not included in any list of points to avoid duplication
            var tempPoints = new List<(Vector2 pos, float curve)> {(_curves[0].startPoint, 0)};
            
            // populate list with all calculated points of all bezier curves
            foreach (var curve in _curves)
            {
                tempPoints.AddRange(curve.CalculatePoints());
            }

            // evenly spaced will have a distance of DISTANCE_UNIT
            var evenlySpacedPoints = new List<RoadPoint>
            {
                new RoadPoint(tempPoints[0].pos, (tempPoints[1].pos - tempPoints[0].pos).normalized, tempPoints[0].curve)
            };
            // save lastPoint to interpolate between calculated points with distance greater than DISTANCE_UNIT
            var lastPoint = tempPoints[0];
            float dstSinceLastEvenPoint = 0;

            foreach(var point in tempPoints)
            {
                // go along the curve to find new point at regular interval
                dstSinceLastEvenPoint += Vector2.Distance(lastPoint.pos, point.pos);

                // checks for overshoot
                while(dstSinceLastEvenPoint >= DISTANCE_UNIT)
                {
                    dstSinceLastEvenPoint -= DISTANCE_UNIT;

                    // goes from lastPoint to this point, with DISTANCE_UNIT length
                    var newEvenlySpacedPoint = lastPoint;
                    newEvenlySpacedPoint.pos = lastPoint.pos + (point.pos - lastPoint.pos).normalized * DISTANCE_UNIT;
                    var newRoadPoint = new RoadPoint(newEvenlySpacedPoint.pos, (point.pos - lastPoint.pos).normalized, newEvenlySpacedPoint.curve);
                    evenlySpacedPoints.Add(newRoadPoint);

                    length += 1f.DistanceUnitsToLength();

                    lastPoint = newEvenlySpacedPoint;
                }

                lastPoint = point;
            }

            _curves.Last().endPoint = lastPoint.pos;

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