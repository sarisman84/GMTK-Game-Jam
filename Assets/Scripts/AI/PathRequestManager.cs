using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Enemies.AI;

public class PathRequestManager
{
    private readonly Queue<PathRequest> _pathRequestQueue = new();
    private PathRequest _currentPathRequest;
    private Pathfinding _pathfinding;
    private bool _isProcessingPath;

    public PathRequestManager(Vector3 center, LayerMask unwalkableMask, Vector2 gridWorldSize, float nodeRadius,
        MonoBehaviour coroutineProcessor)
    {
        _pathfinding = new Pathfinding(center, unwalkableMask, gridWorldSize, nodeRadius, coroutineProcessor, this);
        _pathRequestManager = this;
    }

    private static PathRequestManager _pathRequestManager;
    public Pathfinding logistics => _pathfinding;


    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        _pathRequestManager._pathRequestQueue.Enqueue(newRequest);
        _pathRequestManager.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!_isProcessingPath && _pathRequestQueue.Count > 0)
        {
            _currentPathRequest = _pathRequestQueue.Dequeue();
            _isProcessingPath = true;
            _pathfinding.StartFindPath(_currentPathRequest.PathStart, _currentPathRequest.PathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        _currentPathRequest.Callback?.Invoke(path, success);
        _isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 PathStart, PathEnd;
        public Action<Vector3[], bool> Callback;


        public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathStart = pathStart;
            PathEnd = pathEnd;
            Callback = callback;
        }
    }
}