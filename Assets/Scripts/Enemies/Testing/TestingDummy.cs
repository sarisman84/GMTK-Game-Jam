using System;
using Managers;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Testing
{
    [RequireComponent(typeof(Rigidbody))]
    public class TestingDummy : MonoBehaviour
    {
        public float speed = 5f;
        public float changeDirInIntervals = 2f;
        private Vector3 currentDirection;

        private float curCountdown = 0;

        private Rigidbody _rigidbody;
        private WeaponController _weaponController;


        public Action<Rigidbody> onOverridingFixedUpdate;
        public Action<WeaponController, Transform> onOverridingWeaponBehaviour;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _weaponController = GetComponent<WeaponController>();
        }

        private void Update()
        {
            curCountdown += Time.deltaTime;

            if (curCountdown >= changeDirInIntervals)
            {
                currentDirection = Random.onUnitSphere;
                curCountdown = 0;
            }


            if (onOverridingWeaponBehaviour == null)
            {
                _weaponController.Aim((GameMaster.SingletonAccess.GetPlayer().transform.position - transform.position)
                    .normalized);
                _weaponController.Shoot(IsInsideDetectionRange(GameMaster.SingletonAccess.GetPlayer(), transform, 15f));
            }
            else
                onOverridingWeaponBehaviour.Invoke(_weaponController, transform);
        }

        public static bool IsInsideDetectionRange(GameObject target, Transform transform, float range)
        {
            float dist = Vector3.Distance(target.transform.position, transform.position);
            return dist < range;
        }

        private void FixedUpdate()
        {
            if (onOverridingFixedUpdate == null)
                _rigidbody.velocity = currentDirection * (speed * 100f * Time.fixedDeltaTime);
            else
                onOverridingFixedUpdate.Invoke(_rigidbody);
        }
    }
}