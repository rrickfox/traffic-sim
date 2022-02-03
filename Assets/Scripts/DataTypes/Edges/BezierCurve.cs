using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public class BezierCurve
    {
        public Vector2 startPoint;
        private Vector2 _controlPoint1;
        private Vector2 _controlPoint2;
        public Vector2 endPoint;
        private enum BezierDegree {Linear, Quadratic, Cubic}
        private BezierDegree _degree;
        // function to calculate a single point on a bezier curve using Bernstein polynomials, set at runtime based on degree
        private Func<float, Vector2> Evaluate;
        private Func<float, Vector2> Derivative1;
        private Func<float, Vector2> Derivative2;
        private Func<float, float> Curvature;

        public BezierCurve(Vector2 startPoint, Vector2 endPoint, Vector2? controlPoint1 = null, Vector2? controlPoint2 = null)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            if(controlPoint1 != null)
            {
                this._controlPoint1 = (Vector2) controlPoint1;
                if(controlPoint2 != null)
                {
                    this._controlPoint2 = (Vector2) controlPoint2;
                    _degree = BezierDegree.Cubic;
                }
                else
                {
                    _degree = BezierDegree.Quadratic;
                }
            }
            else
                _degree = BezierDegree.Linear;

            setFunctions();
        }

        private void setFunctions()
        {
            switch(_degree)
            {
                case BezierDegree.Linear:
                {
                    Evaluate = (t) => (1 - t) * startPoint + t * endPoint;
                    Derivative1 = (t) => endPoint - startPoint;
                    Derivative2 = (t) => Vector2.zero;
                    Curvature = (t) => 0;
                    break;
                }
                case BezierDegree.Quadratic:
                {
                    Evaluate = (t) => (1 - 2*t + t*t) * startPoint
                        + (2*t - 2*t*t) * _controlPoint1
                        + (t*t) * endPoint;
                    Derivative1 = (t) => (2*t - 2) * startPoint
                        + (-4*t + 2) * _controlPoint1
                        + (2*t) * endPoint;
                    Derivative2 = (t) => (2) * startPoint
                        + (-4) * _controlPoint1
                        + (2) * endPoint;
                    Curvature = (t) => { var a = Derivative1(t); return a.det(Derivative2(t)) / Mathf.Pow(a.magnitude, 3); };
                    break;
                }
                case BezierDegree.Cubic:
                {
                    Evaluate = (t) => (1 - 3*t + 3*t*t - t*t*t) * startPoint
                        + (3*t - 6*t*t + 3*t*t*t) * _controlPoint1
                        + (3*t*t - 3*t*t*t) * _controlPoint2
                        + (t*t*t) * endPoint;
                    Derivative1 = (t) => (-3*t*t + 6*t - 3) * startPoint
                        + (9*t*t - 12*t + 3) * _controlPoint1
                        + (-9*t*t + 6*t) * _controlPoint2
                        + (6*t) * endPoint;
                    Derivative2 = (t) => (-6*t + 6) * startPoint
                        + (18*t - 12) * _controlPoint1
                        + (-18*t + 6) * _controlPoint2
                        + (6*t) * endPoint;
                    Curvature = (t) => { var a = Derivative1(t); return a.det(Derivative2(t)) / Mathf.Pow(a.magnitude, 3); };
                    break;
                }
            }
        }

        // calculates points on a bezier curve, not necessarily with same distance
        public IEnumerable<Vector2> CalculatePoints()
        {
            for(var i = BEZIER_RESOLUTION; i <= 1; i += BEZIER_RESOLUTION)
            {
                yield return Evaluate(i);
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
                    return new BezierCurve(endPoint, startPoint, _controlPoint1, null);
                case BezierDegree.Cubic:
                    return new BezierCurve(endPoint, startPoint, _controlPoint2, _controlPoint1);
                default:
                    return null;
            }
        }
    }
}