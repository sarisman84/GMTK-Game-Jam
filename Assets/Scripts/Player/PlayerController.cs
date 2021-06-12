using System;
using General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private WeaponController _weaponController;
        private PossessionManager _possessionManager;
        private HealthModifier _health;

        public Transform model;
        public InputActionAsset inputAsset;
        public float maxMovementSpeed;
        public float accelerationSpeed;

        private void Awake()
        {
            CustomInput.ImportAsset(inputAsset,
                CustomInput.DirectionalKeys,
                "DirectionalShot",
                "Possess",
                "Fire",
                "ScrollSelect",
                "Select_One",
                "Select_Two",
                "Select_Three",
                "Select_Four",
                "Select_Five",
                "Select_Six",
                "Select_Seven",
                "Select_Eight",
                "Select_Nine");
            _rigidbody = GetComponent<Rigidbody>();
            _weaponController = GetComponent<WeaponController>();
            _possessionManager = GetComponent<PossessionManager>();
            _health = GetComponent<HealthModifier>();
            _possessionManager.ONPossessionEvent += enemy =>
                _weaponController.AddWeaponToLibrary(enemy.GetComponent<WeaponController>().weaponLibrary[0]);
        }

        private void Update()
        {
            if (_weaponController)
            {
                int val = (int) CustomInput.GetSingleDirectionInput("ScrollSelect");
                val = val == 0 ? 0 : (int) Mathf.Sign(val);
                if (val != 0)
                    _weaponController.SelectWeapon(_weaponController.CurrentWeapon + val);
                _weaponController.Aim(CustomInput.GetDirectionalInput("DirectionalShot"));
                _weaponController.Shoot(CustomInput.GetButton("Fire"));
            }

            if (_possessionManager)
            {
                _possessionManager.ShootPossessionShot(CustomInput.GetButton("Possess"));
            }

            Vector3 movementDir = _rigidbody.velocity.normalized;
            model.rotation = Quaternion.LookRotation(movementDir, Vector3.up);
        }

        private void FixedUpdate()
        {
            Vector2 rawInput = CustomInput.GetDirectionalInput(CustomInput.DirectionalKeys);


            if (rawInput != Vector2.zero)
            {
                rawInput *= accelerationSpeed * Time.fixedDeltaTime;
                _rigidbody.velocity += new Vector3(rawInput.x, 0, rawInput.y);
                _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxMovementSpeed);
            }
            else
                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, 0.55f);
        }


        private void OnDisable()
        {
            CustomInput.SetInputActive(false);
        }
    }
}