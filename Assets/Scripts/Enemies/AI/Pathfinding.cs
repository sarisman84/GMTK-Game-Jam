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
        private PathfindingManager _requestManager;
        private PathfindingArea _area;

        public PathfindingArea area => _area;
        private MonoBehaviour _coroutineOwner;

        public Pathfinding(Vector3 center, LayerMask unwalkableMask, Vector2 gridWorldSize, float nodeRadius,
            MonoBehaviour coroutineOwner, PathfindingManager manager)
        {
            _area = new PathfindingArea(center, unwalkableMask, gridWorldSize, nodeRadius);
            _coroutineOwner = coroutineOwner;
            _requestManager = manager;
        }


        IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, Func<List<Node>, Vector3[]> weightCallback)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3[] waypoints = new Vector3[0];
            bool pathSuccess = false;

            Node startNode = _area.GetNodeFromWorldPosition(startPos);
            Node targetNode = _area.GetNodeFromWorldPosition(targetPos);

            if (startNode.Walkable && targetNode.Walkable)
            {
                Heap<Node> openSet = new Heap<Node>(_area.maxSize);
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node node = openSet.RemoveFirst();
                    closedSet.Add(node);

                    if (node == targetNode)
                    {
                        sw.Stop();
                        pathSuccess = true;
                        break;
                    }

                    foreach (Node neighbour in _area.GetNeighbours(node))
                    {
                        if (!neighbour.Walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                        if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = node;

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
                waypoints = RetracePath(startNode, targetNode, weightCallback);
            }

            _requestManager.FinishedProcessingPath(waypoints, pathSuccess);
        }

        Vector3[] RetracePath(Node startNode, Node endNode, Func<List<Node>, Vector3[]> weightCallback)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }


            Vector3[] waypoints = weightCallback?.Invoke(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        public static Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].XGridPos - path[i].XGridPos,
                    path[i - 1].YGridPos - path[i].YGridPos);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].WorldPosition);
                    directionOld = directionNew;
                }
            }

            return waypoints.ToArray();
        }

        int GetDistance(Node a, Node b)
        {
            int distX = Mathf.Abs(a.XGridPos - b.XGridPos);
            int distY = Mathf.Abs(a.YGridPos - b.YGridPos);

            if (distX > distY)
            {
                return 14 * distY + 10 * (distX - distY);
            }

            return 14 * distX + 10 * (distY - distX);
        }

        public void StartFindPath(Vector3 pathStart, Vector3 pathEnd, Func<List<Node>, Vector3[]> weightCallback = null)
        {
            _coroutineOwner.StartCoroutine(FindPath(pathStart, pathEnd, weightCallback ?? SimplifyPath));
        }
    }
}