using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private WeaponController _weaponController;


        public Transform model;
        public InputActionAsset inputAsset;
        public float maxMovementSpeed;
        public float accelerationSpeed;

        private void Awake()
        {
            CustomInput.ImportAsset(inputAsset, CustomInput.DirectionalKeys, "DirectionalShot", "Possess", "Fire");
            _rigidbody = GetComponent<Rigidbody>();
            _weaponController = GetComponent<WeaponController>();
        }

        private void Update()
        {
            if (_weaponController)
            {
                _weaponController.Aim(CustomInput.GetDirectionalInput("DirectionalShot"));
                _weaponController.Shoot(CustomInput.GetButton("Fire"));
            }

            Vector3 movementDir = _rigidbody.velocity.normalized;
            model.rotation = Quaternion.LookRotation(movementDir, Vector3.up);
            
        }

        private void FixedUpdate()
        {
            Vector2 rawInput = CustomInput.GetDirectionalInput(CustomInput.DirectionalKeys);
            rawInput *= accelerationSpeed * Time.fixedDeltaTime;
            _rigidbody.velocity += new Vector3(rawInput.x, 0, rawInput.y);
            _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxMovementSpeed);
        }


        private void OnDisable()
        {
            CustomInput.SetInputActive(false);
        }
    }
}