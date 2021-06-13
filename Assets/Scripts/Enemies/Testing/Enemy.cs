using System;
using Managers;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Testing
{
    public class Enemy : BaseEnemy
    {
        public float speed = 5f;
        public float changeDirInIntervals = 2f;
        private Vector3 _currentDirection;

        private float _curCountdown;


        protected override void DefaultWeaponBehaviour()
        {
            WeaponController.Aim((GameMaster.singletonAccess.playerObject.transform.position - transform.position)
                .normalized);
            WeaponController.Shoot(IsInsideDetectionRange(GameMaster.singletonAccess.playerObject, transform, 15f));
        }

        protected override void DefaultRigidbodyBehaviour()
        {
            Rigidbody.velocity = _currentDirection * (speed * 100f * Time.fixedDeltaTime);
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

        public static bool IsInsideDetectionRange(GameObject target, Transform transform, float range)
        {
            float dist = Vector3.Distance(target.transform.position, transform.position);
            return dist < range;
        }
    }
}