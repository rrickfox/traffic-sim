using System.Collections.Generic;
using UnityEngine;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public class BezierCurve
    {
        public Vector2 startPoint;
        public Vector2 controlPoint1;
        public Vector2 controlPoint2;
        public Vector2 endPoint;
        private enum BezierDegree {Linear, Quadratic, Cubic}
        private BezierDegree _degree;

        public BezierCurve(Vector2 startPoint, Vector2 endPoint, Vector2? controlPoint1 = null, Vector2? controlPoint2 = null)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            _degree = BezierDegree.Linear;
            if(controlPoint1 != null)
            {
                this.controlPoint1 = (Vector3) controlPoint1;
                _degree = BezierDegree.Quadratic;
                if(controlPoint2 != null)
                {
                    this.controlPoint2 = (Vector3) controlPoint2;
                    _degree = BezierDegree.Cubic;
                }
            }
        }

        // calculates points on a bezier curve, not necessarily with same distance
        public IEnumerable<Vector2> CalculatePoints()
        {
            for(var i = BEZIER_RESOLUTION; i <= 1; i += BEZIER_RESOLUTION)
                yield return Evaluate(i);
        }

        // function to calculate a single point on a bezier curve
        private Vector2 Evaluate(float t)
        {
            switch(_degree)
            {
                case BezierDegree.Linear:
                {
                    return Vector2.Lerp(startPoint, endPoint, t);
                }
                case BezierDegree.Quadratic:
                {
                    var p0 = Vector2.Lerp(startPoint, controlPoint1, t);
                    var p1 = Vector2.Lerp(controlPoint1, endPoint, t);
                    
                    return Vector2.Lerp(p0, p1, t);
                }
                case BezierDegree.Cubic:
                {
                    var p00 = Vector2.Lerp(startPoint, controlPoint1, t);
                    var p01 = Vector2.Lerp(controlPoint1, controlPoint2, t);
                    var p02 = Vector2.Lerp(controlPoint2, endPoint, t);

                    var p10 = Vector2.Lerp(p00, p01, t);
                    var p11 = Vector2.Lerp(p01, p02, t);

                    return Vector2.Lerp(p10, p11, t);
                }
                default:
                    return Vector2.zero;
            }
        }

        // invert the direction of this curve
        public BezierCurve Revert()
        {
            switch(_degree)
            {
                case BezierDegree.Linear:
                    return new BezierCurve(endPoint, startPoint, null, null);
                case BezierDegree.Quadratic:
                    return new BezierCurve(endPoint, startPoint, controlPoint1, null);
                case BezierDegree.Cubic:
                    return new BezierCurve(endPoint, startPoint, controlPoint2, controlPoint1);
                default:
                    return null;
            }
        }
    }
}