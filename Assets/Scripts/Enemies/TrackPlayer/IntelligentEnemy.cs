using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Enemies.AI;
using Enemies.Testing;
using Player;
using UnityEngine;

public class IntelligentEnemy : BaseEnemy
{
    // Start is called before the first frame update
    public float pathRefreshRate = 0.5f;


    private float _curRefreshRate = 0;
    private Coroutine _moveTowardsPath;
    private Vector3 _currentPathPos;
    private float _distFromTarget;


    protected override void DefaultPathfind()
    {
        _curRefreshRate += Time.deltaTime;

        if (_curRefreshRate >= pathRefreshRate)
        {
            PathfindingManager.RequestPath(transform.position, currentTarget.transform.position,
                (path, success) => OnPathFound(path, success));
            _curRefreshRate = 0;
        }
    }


    private void OnDrawGizmos()
    {
        if (Path != null)
        {
            Path.DrawWithGizmos(currentTarget.GetComponent<PlayerController>() ? Color.black : Color.cyan);
        }
    }
}