using UnityEngine;
using System.Collections.Generic;

namespace DataTypes
{
    public class RoadShape
    {
        private List<BezierCurve> _curves;
        public RoadPoint[] points;
        public float length = 0;

        // create RoadShape from list of curves
        public RoadShape(List<BezierCurve> curves)
        {
            _curves = curves;

            CalculateEvenlySpacedPoints();
            CalculateLength();
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
                }

                lastPoint = point;
            }

            points = evenlySpacedPoints.ToArray();
        }

        // calculate length of roadShape
        public void CalculateLength()
        {
            var lastPoint = points[0];
            foreach(var point in points)
            {
                length += Vector2.Distance(lastPoint.position, point.position);
                lastPoint = point;
            }

            length = length / CONSTANTS.DISTANCE_UNIT;
        }
    }
}