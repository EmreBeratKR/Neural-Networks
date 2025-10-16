using UnityEngine;

namespace Physics
{
    public static class Collisions2D
    {
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