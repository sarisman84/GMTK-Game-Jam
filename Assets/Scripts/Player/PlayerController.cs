﻿using System;
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

        public Transform model;
        public InputActionAsset inputAsset;
        public float maxMovementSpeed;
        public float accelerationSpeed;

        private void Awake()
        {
            CustomInput.ImportAsset(inputAsset, CustomInput.DirectionalKeys, "DirectionalShot", "Possess", "Fire");
            _rigidbody = GetComponent<Rigidbody>();
            _weaponController = GetComponent<WeaponController>();
            _possessionManager = GetComponent<PossessionManager>();
        }

        private void Update()
        {
            if (_weaponController)
            {
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