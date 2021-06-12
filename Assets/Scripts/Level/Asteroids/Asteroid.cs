using System;
using General;
using UnityEngine;

namespace Level.Asteroids
{
    [RequireComponent(typeof(Rigidbody))]
    public class Asteroid : MonoBehaviour
    {
        public Rigidbody Rigidbody { get; private set; }


        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.GetComponent<HealthModifier>() is { } modifier)
            {
                modifier.TakeDamage(15f);
            }
        }

        private float countdown = 0;

        private void Update()
        {
            countdown += Time.deltaTime;

            if (countdown >= 5f)
            {
                gameObject.SetActive(false);
                countdown = 0;
            }
        }
    }
}