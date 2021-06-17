using System.Collections;
using UnityEngine;
using System.Collections.Generic;


namespace Enemies.AI
{
    public class Pathfinding
    {
        private PathfindingGrid _grid;

        public PathfindingGrid Grid => _grid;

        public Pathfinding(Vector3 center, LayerMask unwalkableMask, Vector2 gridWorldSize, float nodeRadius)
        {
            _grid = new PathfindingGrid(center, unwalkableMask, gridWorldSize, nodeRadius);
        }


        public void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = _grid.GetNodeFromWorldPosition(startPos);
            Node targetNode = _grid.GetNodeFromWorldPosition(targetPos);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node node = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                    {
                        if (openSet[i].hCost < node.hCost)
                            node = openSet[i];
                    }
                }

                openSet.Remove(node);
                closedSet.Add(node);

                if (node == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return;
                }

                foreach (Node neighbour in _grid.GetNeighbours(node))
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
                    }
                }
            }
        }

        void RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();

            _grid.currentPath = path;
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
    }
}