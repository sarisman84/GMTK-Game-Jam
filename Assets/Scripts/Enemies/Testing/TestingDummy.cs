using System;
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


        public Action<Rigidbody> onOverridingFixedUpdate;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            curCountdown += Time.deltaTime;

            if (curCountdown >= changeDirInIntervals)
            {
                currentDirection = Random.onUnitSphere;
                curCountdown = 0;
            }
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