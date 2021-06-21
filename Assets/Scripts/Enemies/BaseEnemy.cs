using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.AI;
using General;
using Managers;
using Player;
using UnityEngine;
using Utility;

namespace Enemies
{
    [RequireComponent(typeof(WeaponController))]
    public abstract class BaseEnemy : MonoBehaviour
    {
        public float attackRange;
        public float detectionRange = 9999;
        protected Action MovementBehaviourEvent;
        protected WeaponController WeaponController;
        protected HealthModifier HealthModifier;
        protected LayerMask TargetLayer;
        
        public bool isPossessed { get; set; }


        public enum TargetType
        {
            Player,
            Enemy
        }

        public abstract float movementSpeed { get; set; }

        public HealthModifier healthManager => HealthModifier;
        public WeaponController weaponController => WeaponController;
        public WeaponController weaponManager => WeaponController;
        public TargetType CurrentTarget { get; private set; }
        public LayerMask CurrentTargetLayer => TargetLayer;
        public event Action<BaseEnemy> ONTargetUpdateEvent;


        public static GameObject GetTargetFromDetectionArea(Transform transform, float detectionRange,
            LayerMask TargetLayer)
        {
            Collider[] colliders = new Collider[3];
            Physics.OverlapSphereNonAlloc(transform.position, detectionRange, colliders, TargetLayer);
            return colliders.ConvertTo(c =>
                {
                    if (!c)
                        return new PotentialTarget();
                    return new PotentialTarget(transform.gameObject, c.gameObject);
                }).ToHeap().RemoveFirst()
                .Target;
        }

        public void SetTarget<T>(T playerControllerTransform) where T : MonoBehaviour
        {
            SetTarget(playerControllerTransform.GetType());
        }

        protected void OnMovementUpdate<TEnemy>(TEnemy enemy) where TEnemy : BaseEnemy
        {
            ONTargetUpdateEvent?.Invoke(enemy);
        }

        protected bool IsSelfUsingCustomMovementUpdate()
        {
            return ONTargetUpdateEvent != null;
        }

        protected void ResetMovementUpdate()
        {
            ONTargetUpdateEvent = null;
        }

        protected void ResetPossessionState()
        {
            isPossessed = false;
        }

        public virtual void SetTarget(Type type)
        {
            if (type == typeof(PlayerController))
            {
                TargetLayer = LayerMask.GetMask("Ally", "Player");
                CurrentTarget = TargetType.Player;
            }
            else if (type == typeof(BaseEnemy))
            {
                TargetLayer = LayerMask.GetMask("Enemy");
                CurrentTarget = TargetType.Enemy;
            }
        }


        protected void UseWeapon(GameObject target)
        {
            if (WeaponController && target && IsInsideDetectionRange(target, transform, attackRange))
            {
                var position = transform.position;
                Vector3 targetDir = (target.transform.position - position).normalized;
                weaponController.Aim(targetDir);
                weaponController.Shoot(!Physics.Raycast(position, targetDir, attackRange,
                    LayerMask.GetMask("Default")));
            }

            weaponController.Aim(transform.forward.normalized);
        }


        protected virtual void Awake()
        {
            WeaponController = GetComponent<WeaponController>();
            HealthModifier = GetComponent<HealthModifier>();
        }


        public static bool IsInsideDetectionRange(GameObject target, Transform transform, float range)
        {
            float dist = Vector3.Distance(target.transform.position, transform.position);
            return dist < range;
        }


        #region HeapImplementationOfGameObject

        class PotentialTarget : IHeapItem<PotentialTarget>
        {
            public GameObject Target;
            public int HeapIndex { get; set; }

            public PotentialTarget(GameObject owner, GameObject target)
            {
                Target = target;
                HeapIndex = Mathf.RoundToInt(Vector3.Distance(owner.transform.position, target.transform.position));
            }

            public PotentialTarget()
            {
                HeapIndex = int.MaxValue;
            }

            public int CompareTo(PotentialTarget other)
            {
                return other.HeapIndex.CompareTo(HeapIndex);
            }
        }

        #endregion
    }
}