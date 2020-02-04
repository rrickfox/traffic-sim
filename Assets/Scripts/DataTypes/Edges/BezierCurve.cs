using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    public class BezierCurve
    {
        public Vector2 startPoint;
        public Vector2 controlPoint;
        public Vector2 endPoint;

        public BezierCurve(Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint)
        {
            this.startPoint = startPoint;
            this.controlPoint = controlPoint;
            this.endPoint = endPoint;
        }

        public IEnumerable<Vector2> CalculatePoints()
        {
            var calculatedPoints = new List<Vector2>();
            var lastPoint = startPoint;

            for(var i = CONSTANTS.BEZIER_RESOLUTION; i <= 1; i += CONSTANTS.BEZIER_RESOLUTION)
            {
                var newPoint = EvaluateQuadratic(i);
                calculatedPoints.Add(newPoint);
                lastPoint = newPoint;
            }

            return calculatedPoints;
        }

        private Vector2 EvaluateQuadratic(float t)
        {
            var p0 = Vector2.Lerp(startPoint, controlPoint, t);
            var p1 = Vector2.Lerp(controlPoint, endPoint, t);
            
            return Vector2.Lerp(p0, p1, t);
        }

        public BezierCurve Revert() => new BezierCurve(endPoint, controlPoint, startPoint);
    }
}