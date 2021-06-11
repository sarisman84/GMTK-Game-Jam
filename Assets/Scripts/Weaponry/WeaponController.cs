using System;
using UnityEngine;

namespace Player
{
    public class WeaponController : MonoBehaviour
    {
        public Transform barrel;
        public float fireRate;
        public GameObject bulletPrefab;

        private float _currentFireRate = 0;
        private Camera _cam;
        private Plane _plane;

        private void Awake()
        {
            _cam = Camera.main;
            _plane = new Plane();
        }

        public void Aim(Vector2 mousePosition)
        {
            _plane.SetNormalAndPosition(transform.up, transform.position);
            Ray ray = _cam.ScreenPointToRay(mousePosition);
            _plane.Raycast(ray, out var dist);

            Vector3 dir = ray.GetPoint(dist) - transform.position;
            dir.Normalize();

            barrel.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        public void Shoot(bool input)
        {
            _currentFireRate += Time.deltaTime;

            if (input && _currentFireRate >= fireRate)
            {
                GameObject clone = Instantiate(bulletPrefab,
                    barrel.transform.position + (barrel.forward.normalized * 3f), barrel.transform.rotation);
                _currentFireRate = 0;
            }
        }
    }
}