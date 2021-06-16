using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private Collider _collider;

        public float lifeDuration = 5f;
        public Rigidbody Rigidbody => _rigidbody;
        public Type currentTarget { get; set; }

        public event Action<Bullet> ONFixedUpdateEvent;
        public event Action<Collider> ONCollisionEnterEvent;


        float currentDur = 0;

        public void SetTriggerActive(bool state)
        {
            _collider.isTrigger = state;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            currentDur += Time.deltaTime;

            if (currentDur >= lifeDuration)
            {
                gameObject.SetActive(false);
                currentDur = 0;
            }
        }

        private void FixedUpdate()
        {
            ONFixedUpdateEvent?.Invoke(this);
        }

        private void OnCollisionEnter(Collision other)
        {
            ONCollisionEnterEvent?.Invoke(other.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            ONCollisionEnterEvent?.Invoke(other);
        }


        private void OnDisable()
        {
            currentDur = 0;
            ONFixedUpdateEvent = null;
            ONCollisionEnterEvent = null;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            transform.localScale = Vector3.one;
            SetTriggerActive(false);
        }
    }
}