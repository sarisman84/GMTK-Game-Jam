using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using Utility;
using Debug = UnityEngine.Debug;

namespace Enemies.AI
{
    public class Pathfinding
    {
        private PathRequestManager _requestManager;
        private PathfindingArea _area;

        public PathfindingArea area => _area;
        private MonoBehaviour _coroutineOwner;

        public Pathfinding(Vector3 center, LayerMask unwalkableMask, Vector2 gridWorldSize, float nodeRadius,
            MonoBehaviour coroutineOwner, PathRequestManager manager)
        {
            _area = new PathfindingArea(center, unwalkableMask, gridWorldSize, nodeRadius);
            _coroutineOwner = coroutineOwner;
            _requestManager = manager;
        }


        public void StartFindPath(Vector3 startPos, Vector3 targetPos)
        {
            _coroutineOwner.StartCoroutine(FindPath(startPos, targetPos));
        }

        IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3[] waypoints = new Vector3[0];
            bool pathSuccess = false;

            Node startNode = _area.GetNodeFromWorldPosition(startPos);
            Node targetNode = _area.GetNodeFromWorldPosition(targetPos);
            startNode.parent = startNode;


            if (startNode.Walkable && targetNode.Walkable)
            {
                Heap<Node> openSet = new Heap<Node>(_area.maxSize);
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        sw.Stop();
                        pathSuccess = true;
                        break;
                    }

                    foreach (Node neighbour in _area.GetNeighbours(currentNode))
                    {
                        if (!neighbour.Walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }

            yield return null;
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
            }

            _requestManager.FinishedProcessingPath(waypoints, pathSuccess);
        }


        Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew =
                    new Vector2(path[i - 1].XGridPos - path[i].XGridPos, path[i - 1].YGridPos - path[i].YGridPos);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].WorldPosition);
                }

                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.XGridPos - nodeB.XGridPos);
            int dstY = Mathf.Abs(nodeA.YGridPos - nodeB.YGridPos);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}