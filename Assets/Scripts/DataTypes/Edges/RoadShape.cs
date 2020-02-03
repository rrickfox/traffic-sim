using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        public RoadShape(List<BezierCurve> curves, RoadPoint[] points, RoadPoint endingPoint, float length)
        {
            _curves = curves;
            this.points = points;
            this.endingPoint = endingPoint;
            this.length = length;
        }

        // calculates points in regular intervals
        private void CalculateEvenlySpacedPoints()
        {
            var tempPoints = new List<Vector2>();

            tempPoints.Add(_curves[0].startPoint);

            foreach (var curve in _curves)
            {
                tempPoints.AddRange(curve.CalculatePoints());
            }

            var evenlySpacedPoints = new List<RoadPoint>();
            evenlySpacedPoints.Add(new RoadPoint(tempPoints[0], (tempPoints[1] - tempPoints[0]).normalized));
            var lastPoint = tempPoints[0];
            var secondLastPoint = tempPoints[0];
            float dstSinceLastEvenPoint = 0;

            foreach(var point in tempPoints)
            {
                dstSinceLastEvenPoint += Vector2.Distance(lastPoint, point);
                
                while(dstSinceLastEvenPoint >= CONSTANTS.DISTANCE_UNIT)
                {
                    var overshootDst = dstSinceLastEvenPoint - CONSTANTS.DISTANCE_UNIT;
                    var newEvenlySpacedPoint = point + (point - lastPoint).normalized * overshootDst;
                    var newRoadPoint = new RoadPoint(newEvenlySpacedPoint, (point - lastPoint).normalized);
                    evenlySpacedPoints.Add(newRoadPoint);
                    dstSinceLastEvenPoint = overshootDst;

                    length++;
                }

                secondLastPoint = lastPoint;
                lastPoint = point;
            }

            length += dstSinceLastEvenPoint / CONSTANTS.DISTANCE_UNIT;
            endingPoint = new RoadPoint(lastPoint, (secondLastPoint - lastPoint).normalized);

            points = evenlySpacedPoints.ToArray();
        }

        public RoadShape Inverse()
        {
            var reverseCurves = _curves.ToList();
            reverseCurves.Reverse();

            foreach (var curve in reverseCurves)
            {
                curve.Revert();
            }

            return new RoadShape(reverseCurves, points.Reverse().ToArray(), points[0], length);
        }
    }
}