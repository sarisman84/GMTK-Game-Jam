using System;
using Managers;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Testing
{
    public class BasicEnemy : BaseEnemy
    {
        public float changeDirInIntervals = 2f;
        private Vector3 _currentDirection;

        private float _curCountdown;


        protected override void DefaultRigidbodyBehaviour()
        {
            Rigidbody.velocity = _currentDirection * (MovementSpeed * Time.fixedDeltaTime);
        }

        protected override void Update()
        {
            _curCountdown += Time.deltaTime;

            if (_curCountdown >= changeDirInIntervals)
            {
                _currentDirection = Random.onUnitSphere;
                _curCountdown = 0;
            }

            base.Update();
        }

     
    }
}