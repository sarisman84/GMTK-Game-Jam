using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        public float speed = 5f;
        public Rigidbody Rigidbody => _rigidbody;

        public event Action<Bullet> ONFixedUpdateEvent;
        public event Action<Collider> ONCollisionEnterEvent;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }


        private void FixedUpdate()
        {
            ONFixedUpdateEvent?.Invoke(this);
        }

        private void OnCollisionEnter(Collision other)
        {
            ONCollisionEnterEvent?.Invoke(other.collider);
        }

        private void OnDestroy()
        {
            ONFixedUpdateEvent = null;
            ONCollisionEnterEvent = null;
        }

        private void OnDisable()
        {
            ONFixedUpdateEvent = null;
            ONCollisionEnterEvent = null;
        }
    }
}