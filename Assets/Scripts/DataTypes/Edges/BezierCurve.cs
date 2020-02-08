using System.Collections.Generic;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;

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

        // calculates points on a bezier curve, not necessarily with same distance
        public IEnumerable<Vector2> CalculatePoints()
        {
            for(var i = BEZIER_RESOLUTION; i <= 1; i += BEZIER_RESOLUTION)
            {
                yield return EvaluateQuadratic(i);
            }
        }

        // function to calculate a single point on a bezier curve
        private Vector2 EvaluateQuadratic(float t)
        {
            var p0 = Vector2.Lerp(startPoint, controlPoint, t);
            var p1 = Vector2.Lerp(controlPoint, endPoint, t);
            
            return Vector2.Lerp(p0, p1, t);
        }

        // invert the direction of this curve
        public BezierCurve Revert() => new BezierCurve(endPoint, controlPoint, startPoint);
    }
}