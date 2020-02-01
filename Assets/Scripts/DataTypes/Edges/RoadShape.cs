using UnityEngine;
using System.Collections.Generic;

namespace DataTypes
{
    public class RoadShape
    {
        private List<BezierCurve> _curves;
        public Vector2[] points;
        public float length = 0;

        public RoadShape(List<BezierCurve> curves)
        {
            _curves = curves;

            CalculateEvenlySpacedPoints();
            CalculateLength();
        }

        private void CalculateEvenlySpacedPoints()
        {
            var tempPoints = new List<Vector2>();

            tempPoints.Add(_curves[0].startPoint);

            foreach (var curve in _curves)
            {
                tempPoints.AddRange(curve.CalculatePoints());
            }

            var evenlySpacedPoints = new List<Vector2>();
            evenlySpacedPoints.Add(tempPoints[0]);
            var lastPoint = evenlySpacedPoints[0];
            float dstSinceLastEvenPoint = 0;

            foreach(var point in tempPoints)
            {
                dstSinceLastEvenPoint += Vector2.Distance(lastPoint, point);
                
                while(dstSinceLastEvenPoint >= CONSTANTS.DISTANCE_UNIT)
                {
                    var overshootDst = dstSinceLastEvenPoint - CONSTANTS.DISTANCE_UNIT;
                    var newEvenlySpacedPoint = point + (point - lastPoint).normalized * overshootDst;
                    evenlySpacedPoints.Add(newEvenlySpacedPoint);
                    dstSinceLastEvenPoint = overshootDst;
                }

                lastPoint = point;
            }

            points = evenlySpacedPoints.ToArray();
        }

        public void CalculateLength()
        {
            var lastPoint = points[0];
            foreach(var point in points)
            {
                length += Vector2.Distance(lastPoint, point);
                lastPoint = point;
            }

            length = length / CONSTANTS.DISTANCE_UNIT;
        }
    }
}