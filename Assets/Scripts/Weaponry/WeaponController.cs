using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class WeaponController : MonoBehaviour
    {
        public Transform barrel;
        public List<WeaponSettings> weaponLibrary;

        private int _currentWeapon = 0;
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

        public void Aim(Vector3 direction)
        {
            barrel.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        public void Shoot(bool input)
        {
            _currentFireRate += Time.deltaTime;

            if (input && _currentFireRate >= weaponLibrary[_currentWeapon].fireRate)
            {
                weaponLibrary[_currentWeapon].OnShoot(barrel);
                _currentFireRate = 0;
            }
        }
    }
}