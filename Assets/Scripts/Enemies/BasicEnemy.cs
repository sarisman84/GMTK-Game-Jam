using System;
using Managers;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Testing
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicEnemy : BaseEnemy
    {
        public float changeDirInIntervals = 2f;
        public float speed;

        private float _curCountdown;
        private Rigidbody _rigidbody;
        private Vector3 _currentDirection;

        public override float movementSpeed
        {
            get => speed;
            set => speed = value;
        }

        private void Update()
        {
            _curCountdown += Time.deltaTime;

            if (_curCountdown >= changeDirInIntervals)
            {
                _currentDirection = Random.onUnitSphere.normalized;
                _curCountdown = 0;
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_currentDirection), 0.7f);
            UseWeapon(GetTargetFromDetectionArea());
        }

        protected override void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            base.Awake();
        }


        private void FixedUpdate()
        {
            _rigidbody.velocity = transform.forward.normalized * (movementSpeed * Time.fixedDeltaTime);
        }
    }
}