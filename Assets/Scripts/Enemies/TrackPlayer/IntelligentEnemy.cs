using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Enemies.Testing;
using UnityEngine;

public class IntelligentEnemy : BaseEnemy
{
    // Start is called before the first frame update
    public float pathRefreshRate = 0.5f;

    private Vector3[] _path;
    private int _targetIndex;

    private float _curRefreshRate = 0;
    private Coroutine _moveTowardsPath;
    private Vector3 _currentPathPos;
    private float _distFromTarget;


    protected override void DefaultRigidbodyBehaviour()
    {
        if (_distFromTarget > attackRange)
            Rigidbody.MovePosition(_currentPathPos);
    }

    protected override void Update()
    {
        _curRefreshRate += Time.deltaTime;

        if (_curRefreshRate >= pathRefreshRate)
        {
            PathfindingManager.RequestPath(transform.position, target.transform.position, OnPathFound);
            _curRefreshRate = 0;
        }

        if (!Physics.Raycast(transform.position, DirectionToTarget, attackRange))
            _distFromTarget = Vector3.Distance(transform.position, target.transform.position);
        base.Update();
    }

    protected virtual void OnPathFound(Vector3[] foundPath, bool isSuccessful)
    {
        if (isSuccessful)
        {
            _path = foundPath;
            if (_moveTowardsPath != null)
                StopCoroutine(_moveTowardsPath);
            _moveTowardsPath = StartCoroutine(FollowPath());
        }
    }

    private IEnumerator FollowPath()
    {
        if (_path != null && _path.Length != 0)
        {
            Vector3 currentWaypoint = _path[0];

            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    _targetIndex++;
                    if (_targetIndex >= _path.Length)
                    {
                        yield break;
                    }

                    currentWaypoint = _path[_targetIndex];
                }

                _currentPathPos =
                    Vector3.MoveTowards(transform.position, currentWaypoint, MovementSpeed * Time.deltaTime);

                yield return null;
            }
        }

        yield return null;
    }


    private void OnDrawGizmos()
    {
        if (_path != null)
        {
            for (int i = _targetIndex; i < _path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(_path[i], Vector3.one);

                if (i == _targetIndex)
                {
                    Gizmos.DrawLine(transform.position, _path[i]);
                }
                else
                {
                    Gizmos.DrawLine(_path[i - 1], _path[i]);
                }
            }
        }
    }
}