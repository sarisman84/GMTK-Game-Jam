using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Enemies.AI;

public class PathfindingManager
{
    private Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
    private PathRequest _currentPathRequest;
    private Pathfinding _pathfinding;
    private bool isProcessingPath;
    public PathfindingManager(Vector3 center, LayerMask unwalkableMask, Vector2 gridWorldSize, float nodeRadius, MonoBehaviour coroutineProcessor)
    {
        _pathfinding = new Pathfinding(center, unwalkableMask, gridWorldSize, nodeRadius, coroutineProcessor, this);
        _pathfindingManager = this;
    }

    private static PathfindingManager _pathfindingManager;
    public Pathfinding logistics => _pathfinding;


    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback, Func<List<Node>, Vector3[]> weightCallback = null)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback, weightCallback);
        _pathfindingManager._pathRequestQueue.Enqueue(newRequest);
        _pathfindingManager.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!isProcessingPath && _pathRequestQueue.Count > 0)
        {
            _currentPathRequest = _pathRequestQueue.Dequeue();
            isProcessingPath = true;
            _pathfinding.StartFindPath(_currentPathRequest.PathStart, _currentPathRequest.PathEnd, _currentPathRequest.WeightCallback);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        _currentPathRequest.Callback?.Invoke(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 PathStart, PathEnd;
        public Action<Vector3[], bool> Callback;
        public Func<List<Node>, Vector3[]> WeightCallback;

        public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback,
            Func<List<Node>, Vector3[]> weightCallback)
        {
            PathStart = pathStart;
            PathEnd = pathEnd;
            Callback = callback;
            WeightCallback = weightCallback;
        }
    }
}
