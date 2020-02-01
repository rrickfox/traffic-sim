using UnityEngine;
using System.Collections.Generic;

namespace DataTypes
{
    public class RoadShape
    {
        private List<BezierCurve> _curves;
        public Vector2[] points;
        public float length;

        public RoadShape(List<BezierCurve> curves)
        {
            _curves = curves;

            points = CalculateEvenlySpacedPoints();
            CalculateLength();
        }

        private Vector2[] CalculateEvenlySpacedPoints()
        {
            var tempPoints = new List<Vector2>();

            tempPoints.Add(_curves[0].startPoint);

            foreach (var curve in _curves)
            {
                tempPoints.AddRange(curve.points);
            }

            var evenlySpacedPoints = new List<Vector2>();
            evenlySpacedPoints.Add(tempPoints[0]);
            var lastPoint = evenlySpacedPoints[0];
            float dstSinceLastEvenPoint = 0;

            foreach(var point in tempPoints)
            {
                while(dstSinceLastEvenPoint >= CONSTANTS.DISTANCE_UNIT)
                {
                    var overshootDst = dstSinceLastEvenPoint - CONSTANTS.DISTANCE_UNIT;
                    var newEvenlySpacedPoint = point + (point - lastPoint).normalized * overshootDst;
                    evenlySpacedPoints.Add(newEvenlySpacedPoint);
                    dstSinceLastEvenPoint = overshootDst;
                }

                lastPoint = point;
            }

            return evenlySpacedPoints.ToArray();
        }

        public void CalculateLength()
        {
            foreach(var curve in _curves)
            {
                length += curve.length;
            }
        }
    }
}