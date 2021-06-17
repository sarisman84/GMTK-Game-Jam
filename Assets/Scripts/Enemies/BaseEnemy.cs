using System;
using General;
using Player;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Rigidbody), typeof(WeaponController))]
    public abstract class BaseEnemy : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;
        public float attackRange;
        public Action<Rigidbody> ONOverridingFixedUpdate;
        public Action<WeaponController, Transform> ONOverridingWeaponBehaviour;
        public Action ONOverridingDeathEvent;

        protected Rigidbody Rigidbody;
        protected WeaponController WeaponController;
        protected HealthModifier HealthModifier;

        public bool IsFlaggedForReset { private get; set; }


        public float MovementSpeed
        {
            get => movementSpeed * 100f;
            set => movementSpeed = value;
        }

        public WeaponController WeaponManager => WeaponController;
        public GameObject target { get; set; }

        protected Vector3 DirectionToTarget => (target.transform.position - transform.position).normalized;


        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            WeaponController = GetComponent<WeaponController>();
            WeaponController.SetDesiredTarget(typeof(PlayerController));
            HealthModifier = GetComponent<HealthModifier>();
        }

        protected virtual void DefaultWeaponBehaviour()
        {
            if (target)
            {
                WeaponController.Aim((target.transform.position - transform.position)
                    .normalized);
                WeaponController.Shoot(IsInsideDetectionRange(target, transform, attackRange));
            }
        }

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
            if (HealthModifier.IsFlaggedForDeath)
            {
                ONOverridingDeathEvent?.Invoke();
            }

            if (IsFlaggedForReset)
            {
                ONOverridingFixedUpdate = null;
                ONOverridingWeaponBehaviour = null;
                IsFlaggedForReset = false;
                ONOverridingDeathEvent = null;
            }
        }

        protected virtual void OnDestroy()
        {
            ONOverridingFixedUpdate = null;
            ONOverridingWeaponBehaviour = null;
        }
        
        public static bool IsInsideDetectionRange(GameObject target, Transform transform, float range)
        {
            float dist = Vector3.Distance(target.transform.position, transform.position);
            return dist < range;
        }
    }
}