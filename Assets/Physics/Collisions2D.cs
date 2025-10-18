using UnityEngine;

namespace Physics
{
    public static class Collisions2D
    {
        public static bool LineSegmentAndAxisAlignedRectIntersection(Vector2 lineSegmentStart, Vector2 lineSegmentEnd, Vector2 rectCenter, Vector2 rectSize, out Vector2 point)
        {
            var halfSize = rectSize * 0.5f;
            var topLeft = new Vector2(rectCenter.x - halfSize.x, rectCenter.y + halfSize.y);
            var topRight = new Vector2(rectCenter.x + halfSize.x, rectCenter.y + halfSize.y);
            var bottomRight = new Vector2(rectCenter.x + halfSize.x, rectCenter.y - halfSize.y);
            var bottomLeft = new Vector2(rectCenter.x - halfSize.x, rectCenter.y - halfSize.y);
            
            var rectTopEdgeIntersects = LineSegmentAndLineSegmentIntersection(lineSegmentStart, lineSegmentEnd, topLeft, topRight, out var rectTopEdgePoint);
            var rectRightEdgeIntersects = LineSegmentAndLineSegmentIntersection(lineSegmentStart, lineSegmentEnd, topRight, bottomRight, out var rectRightEdgePoint);
            var rectBottomEdgeIntersects = LineSegmentAndLineSegmentIntersection(lineSegmentStart, lineSegmentEnd, bottomLeft, bottomRight, out var rectBottomEdgePoint);
            var rectLeftEdgeIntersects = LineSegmentAndLineSegmentIntersection(lineSegmentStart, lineSegmentEnd, bottomLeft, topLeft, out var rectLeftEdgePoint);

            var closestSqrDistance = float.PositiveInfinity;
            var intersects = false;
            point = Vector2.zero;

            if (rectTopEdgeIntersects)
            {
                intersects = true;
                var sqrDistance = Vector2.SqrMagnitude(lineSegmentStart - rectTopEdgePoint);

                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    point = rectTopEdgePoint;
                }
            }
            
            if (rectRightEdgeIntersects)
            {
                intersects = true;
                var sqrDistance = Vector2.SqrMagnitude(lineSegmentStart - rectRightEdgePoint);

                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    point = rectRightEdgePoint;
                }
            }
            
            if (rectBottomEdgeIntersects)
            {
                intersects = true;
                var sqrDistance = Vector2.SqrMagnitude(lineSegmentStart - rectBottomEdgePoint);

                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    point = rectBottomEdgePoint;
                }
            }
            
            if (rectLeftEdgeIntersects)
            {
                intersects = true;
                var sqrDistance = Vector2.SqrMagnitude(lineSegmentStart - rectLeftEdgePoint);

                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    point = rectLeftEdgePoint;
                }
            }
            
            return intersects;
        }
        
        public static bool LineSegmentAndLineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 point)
        {
            point = Vector2.zero;
            var denominator = (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x);
            if (Mathf.Abs(denominator) < Mathf.Epsilon) return false;
            var t = ((p1.x - p3.x) * (p3.y - p4.y) - (p1.y - p3.y) * (p3.x - p4.x)) / denominator;
            var u = -((p1.x - p2.x) * (p1.y - p3.y) - (p1.y - p2.y) * (p1.x - p3.x)) / denominator;
            if (t is >= 0 and <= 1 && u is >= 0 and <= 1)
            {
                point = new Vector2(p1.x + t * (p2.x - p1.x), p1.y + t * (p2.y - p1.y));
                return true;
            }
            return false;
        }
        
        public static bool CheckCircleAndRotatedRect(Vector2 circleCenter, float circleRadius, Vector2 rectCenter, Vector2 rectSize, float rotation)
        {
            var radians = rotation * Mathf.Deg2Rad;
            var cos = Mathf.Cos(radians);
            var sin = Mathf.Sin(radians);
            var xAxis = new Vector2(cos, sin);
            var yAxis = new Vector2(-sin, cos);
            var centerDelta = circleCenter - rectCenter;
            var virtualCenterDelta = new Vector2(Vector2.Dot(centerDelta, xAxis), Vector2.Dot(centerDelta, yAxis));
            var virtualRectCenter = rectSize * 0.5f;
            var virtualCircleCenter = virtualRectCenter + virtualCenterDelta;

            return CheckCircleAndAxisAlignedRect(virtualCircleCenter, circleRadius, virtualRectCenter, rectSize);
        }

        public static bool CheckCircleAndAxisAlignedRect(Vector2 circleCenter, float circleRadius, Vector2 rectCenter, Vector2 rectSize)
        {
            var halfSize = rectSize * 0.5f;
            var xMin = rectCenter.x - halfSize.x;
            var xMax = rectCenter.x + halfSize.x;
            var yMin = rectCenter.y - halfSize.y;
            var yMax = rectCenter.y + halfSize.y;
            Vector2 closestPoint;

            if (circleCenter.x <= xMin)
            {
                if (circleCenter.y <= yMin)
                {
                    closestPoint = new Vector2(xMin, yMin);
                }
                
                else if (circleCenter.y <= yMax)
                {
                    closestPoint = new Vector2(xMin, circleCenter.y);
                }

                else
                {
                    closestPoint = new Vector2(xMin, yMax);
                }
            }
            
            else if (circleCenter.x <= xMax)
            {
                if (circleCenter.y <= yMin)
                {
                    closestPoint = new Vector2(circleCenter.x, yMin);
                }
                
                else if (circleCenter.y <= yMax)
                {
                    return true;
                }

                else
                {
                    closestPoint = new Vector2(circleCenter.x, yMax);
                }
            }

            else
            {
                if (circleCenter.y <= yMin)
                {
                    closestPoint = new Vector2(xMax, yMin);
                }
                
                else if (circleCenter.y <= yMax)
                {
                    closestPoint = new Vector2(xMax, circleCenter.y);
                }

                else
                {
                    closestPoint = new Vector2(xMax, yMax);
                }
            }

            var sqrDistance = Vector2.SqrMagnitude(closestPoint - circleCenter);
            var sqrRadius = circleRadius * circleRadius;

            return sqrDistance <= sqrRadius;
        }
    }
}