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
     

        private float _curCountdown;


        protected override void DefaultPathfind()
        {
            _curCountdown += Time.deltaTime;

            if (_curCountdown >= changeDirInIntervals)
            {
                currentDirection = Random.onUnitSphere;
                _curCountdown = 0;
            }
        }
    }
}