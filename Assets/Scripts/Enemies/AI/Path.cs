using UnityEngine;

namespace Enemies.AI
{
    public class Path
    {
        public readonly Vector3[] lookPoints;
        public readonly Line[] turnBoundaries;
        public readonly int finishLineIndex;

        public Path(Vector3[] waypoints, Vector3 startPos, float turnDistance)
        {
            lookPoints = waypoints;
            turnBoundaries = new Line[lookPoints.Length];
            finishLineIndex = turnBoundaries.Length - 1;
            Vector2 previousPoint = V3ToV2(startPos);

            for (int i = 0; i < lookPoints.Length; i++)
            {
                Vector2 currentPoint = V3ToV2(lookPoints[i]);
                Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
                Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDistance;
                
                turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDistance);
                previousPoint = turnBoundaryPoint;
            }
        }

        Vector2 V3ToV2(Vector3 v3)
        {
            return new Vector3(v3.x, v3.z);
        }

        public void DrawWithGizmos()
        {
            Gizmos.color = Color.black;
            foreach (var lookPoint in lookPoints)
            {
                Gizmos.DrawCube(lookPoint + Vector3.up, Vector3.one);
            }

            Gizmos.color = Color.white;

            foreach (var l in turnBoundaries)
            {
                l.DrawWithGizmos(10);
            }
        }
    }
}