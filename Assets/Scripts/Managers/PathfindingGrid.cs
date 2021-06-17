using System.Collections;
using System.Collections.Generic;
using Enemies.AI;
using UnityEngine;

public class PathfindingGrid
{
    private Node[,] _grid;
    private LayerMask _unwalkableMask;
    private Vector2 _gridWorldSize;
    private float _nodeRadius;

    private float _nodeDiameter;
    private int _gridSizeX, _gridSizeY;
    private Vector3 position;

    public PathfindingGrid(Vector3 center, LayerMask unwalkableMask, Vector2 gridWorldSize, float nodeRadius)
    {
        _unwalkableMask = unwalkableMask;
        _gridWorldSize = gridWorldSize;
        _nodeRadius = nodeRadius;

        _nodeDiameter = nodeRadius * 2f;

        _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);
        position = center;
        UpdateGrid();
    }

    public List<Node> currentPath { private get; set; }

    public void UpdateGrid()
    {
        _grid = new Node[_gridSizeX, _gridSizeY];
        Vector3 worldButtomLeftPos =
            position - Vector3.right * _gridWorldSize.x / 2 - Vector3.forward * _gridWorldSize.y / 2;

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPoint = worldButtomLeftPos + Vector3.right * (x * _nodeDiameter + _nodeRadius) +
                                     Vector3.forward * (y * _nodeDiameter + _nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, _nodeRadius, _unwalkableMask));
                _grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.XGridPos + x;
                int checkY = node.YGridPos + y;
                if (IsPositionInBounds(checkX, checkY))
                    neighbours.Add(_grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    private bool IsPositionInBounds(int x, int y)
    {
        return x >= 0 && x < _gridSizeX && y >= 0 && y < _gridSizeY;
    }


    public Node GetNodeFromWorldPosition(Vector3 position)
    {
        float percentX = (position.x / _gridWorldSize.x) + 0.5f;
        float percentY = (position.z / _gridWorldSize.y) + 0.5f;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);


        int x = Mathf.FloorToInt(Mathf.Min(_gridSizeX * percentX, _gridSizeX - 1));
        int y = Mathf.FloorToInt(Mathf.Min(_gridSizeY * percentY, _gridSizeY - 1));

        return _grid[x, y];
    }

    /// <summary>
    /// Draws a gizmo to visualise the grid.
    /// </summary>
    public void DrawGrid()
    {
        Gizmos.DrawWireCube(position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));
        if (_grid != null)
        {
            foreach (var node in _grid)
            {
                Gizmos.color = (node.Walkable) ? Color.white : Color.red;
                if (currentPath != null && currentPath.Contains(node))
                    Gizmos.color = Color.black;
                Gizmos.DrawCube(node.WorldPosition, Vector3.one * (_nodeDiameter - 0.1f));
            }
        }
    }
}