using UnityEngine;

namespace Enemies.AI
{
    public struct Line
    {
        private const float verticalLineGradient = 1e5f;

        private float gradient;
        private float y_intercept;

        private Vector2 pointOnLine_1, pointOnLine_2;

        private float gradientPerpedicular;

        private bool approachSide;

        public Line(Vector2 pointOnLine, Vector2 pointPerpendicularity)
        {
            float dx = pointOnLine.x - pointPerpendicularity.x;
            float dy = pointOnLine.y - pointPerpendicularity.y;

            gradientPerpedicular = dx == 0 ? verticalLineGradient : dy / dx;
            gradient = gradientPerpedicular == 0 ? verticalLineGradient : -1 / gradientPerpedicular;
            y_intercept = pointOnLine.y - gradient * pointOnLine.x;

            pointOnLine_1 = pointOnLine;
            pointOnLine_2 = pointOnLine + new Vector2(1, gradient);

            approachSide = false;
            approachSide = GetSide(pointPerpendicularity);
        }

        bool GetSide(Vector2 p)
        {
            return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) >
                   (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
        }

        public bool HasCrossedLine(Vector2 p)
        {
            return GetSide(p) != approachSide;
        }

        public void DrawWithGizmos(float length)
        {
            Vector3 lineDirection = new Vector3(1, 0, gradient).normalized;
            Vector3 lineCenter = new Vector3(pointOnLine_1.x, 0, pointOnLine_1.y)
                                 + Vector3.up;

            Gizmos.DrawLine(lineCenter - lineDirection * length / 2f, lineCenter + lineDirection * length / 2f);
        }
    }
}