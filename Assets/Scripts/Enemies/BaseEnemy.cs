using System;
using Player;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Rigidbody), typeof(WeaponController))]
    public abstract class BaseEnemy : MonoBehaviour
    {
        public Action<Rigidbody> ONOverridingFixedUpdate;
        public Action<WeaponController, Transform> ONOverridingWeaponBehaviour;

        protected Rigidbody Rigidbody;
        protected WeaponController WeaponController;
        
        public bool IsFlaggedForReset { private get; set; }
        public float MovementSpeed { get; set; }

        public WeaponController WeaponManager => WeaponController;

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            WeaponController = GetComponent<WeaponController>();
            WeaponController.SetDesiredTarget(typeof(PlayerController));
        }

        protected abstract void DefaultWeaponBehaviour();
        protected abstract void DefaultRigidbodyBehaviour();

        protected virtual void Update()
        {
            if (ONOverridingWeaponBehaviour == null)
            {
                DefaultWeaponBehaviour();
            }
            else
                ONOverridingWeaponBehaviour.Invoke(WeaponController, transform);
        }

        protected void FixedUpdate()
        {
            if (ONOverridingFixedUpdate == null)
                DefaultRigidbodyBehaviour();
            else
                ONOverridingFixedUpdate.Invoke(Rigidbody);
        }


        protected virtual void OnDisable()
        {
            if (IsFlaggedForReset)
            {
                ONOverridingFixedUpdate = null;
                ONOverridingWeaponBehaviour = null;
                IsFlaggedForReset = false;
            }
           
        }

        protected virtual void OnDestroy()
        {
            ONOverridingFixedUpdate = null;
            ONOverridingWeaponBehaviour = null;
        }
    }
}