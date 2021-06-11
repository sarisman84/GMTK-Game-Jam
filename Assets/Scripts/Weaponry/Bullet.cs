using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        public float velocity = 5f;
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }


        private void FixedUpdate()
        {
            _rigidbody.velocity = transform.forward * (velocity * 100f * Time.fixedDeltaTime);
        }
    }
}