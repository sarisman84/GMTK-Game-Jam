using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Enemies.AI;
using System.Threading;

public class PathRequestManager
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;
    bool isProcessingPath;
    private Pathfinding _pathfinding;


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
        if (_pathRequestManager != null)
        {
            _pathRequestManager.pathRequestQueue.Enqueue(newRequest);
            _pathRequestManager.TryProcessNext();
        }
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            _pathfinding.StartFindPath(currentPathRequest.PathStart, currentPathRequest.PathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.Callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }
}

public struct PathRequest
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