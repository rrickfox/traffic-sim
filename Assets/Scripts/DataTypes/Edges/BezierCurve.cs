using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    public class BezierCurve
    {
        public Vector2 startPoint;
        public Vector2 controlPoint;
        public Vector2 endPoint;

        public float length = 0;
        public List<Vector2> points;

        public BezierCurve(Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint)
        {
            this.startPoint = startPoint;
            this.controlPoint = controlPoint;
            this.endPoint = endPoint;

            points = CalculatePoints();
        }

        public List<Vector2> CalculatePoints()
        {
            List<Vector2> calculatedPoints = new List<Vector2>();
            var lastPoint = startPoint;

            for(float i = CONSTANTS.BEZIER_RESOLUTION; i <= 1; i += CONSTANTS.BEZIER_RESOLUTION)
            {
                var newPoint = EvaluateQuadratic(i);
                length += Vector2.Distance(newPoint, lastPoint);
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
    }
}