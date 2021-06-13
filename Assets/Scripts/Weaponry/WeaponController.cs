using System;
using System.Collections.Generic;
using Player.HUD;
using UnityEngine;
using Utility.Attributes;

namespace Player
{
    public class WeaponController : MonoBehaviour
    {
        public Transform barrel;
        [Expose] public List<WeaponSettings> weaponLibrary;
        public float weaponSwapDelay = 0.25f;

        private int _currentWeapon = 0;
        private float _currentFireRate = 0;
        private float _currDelay;
        private Camera _cam;
        private Plane _plane;
        private WeaponDisplayer _displayer;
        public Type CurTarget { get; private set; }


        public int CurrentWeapon => _currentWeapon;

        private void Awake()
        {
            _cam = Camera.main;
            _plane = new Plane();
            _displayer = GetComponent<WeaponDisplayer>();
            SelectWeapon(_currentWeapon);
        }

        private void Update()
        {
            _currDelay += Time.deltaTime;
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
                weaponLibrary[_currentWeapon].OnShoot(barrel, this);
                _currentFireRate = 0;
            }
        }


        public void AddWeaponToLibrary(WeaponSettings weaponSettings)
        {
            if (weaponSettings == null) return;
            if (_currDelay >= weaponSwapDelay)
            {
                if (!weaponLibrary.Contains(weaponSettings))
                {
                    weaponLibrary.Add(weaponSettings);
                    _currentWeapon = weaponLibrary.Count - 1;
                    _currDelay = 0;
                }
            }
        }

        public void SelectWeapon(int selection)
        {
            if (selection < 0 || selection >= weaponLibrary.Count) return;
            if (_displayer)
                _displayer.OnWeaponSelection();
            _currentWeapon = selection;
        }

        public void SetDesiredTarget(Type target)
        {
            CurTarget = target;
        }

        public void ResetWeaponLibrary()
        {
            WeaponSettings firstWeapon = weaponLibrary[0];
            weaponLibrary = new List<WeaponSettings>();
            weaponLibrary.Add(firstWeapon);
            _currentWeapon = 0;
        }
    }
}