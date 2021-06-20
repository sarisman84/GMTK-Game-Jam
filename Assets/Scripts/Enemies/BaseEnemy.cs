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
    [RequireComponent(typeof(Rigidbody), typeof(WeaponController))]
    public abstract class BaseEnemy : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;
        public float attackRange;
        public float turnDistance = 5;
        public float turnSpeed = 3;
        public float speedPercent = 5;
        public float stoppingDst = 10;

        public event Action ONOverridingDeathEvent;
        public event Action ONOverridingWeaponBehaviour;
        public event Action ONOverridingPathfinding;


        protected Rigidbody Rigidbody;
        protected WeaponController WeaponController;
        protected HealthModifier HealthModifier;

        protected Path Path;
        public Rigidbody rigidBody => Rigidbody;
        public WeaponController weaponController => WeaponController;

        public Vector3 currentDirection;

        public bool isFlaggedForReset { private get; set; }


        public float speed
        {
            get => movementSpeed * 100f;
            set => movementSpeed = value;
        }

        public WeaponController weaponManager => WeaponController;


        protected GameObject currentTarget => GetTargetFromDetectionArea(transform.position, attackRange * 100);

        private GameObject GetTargetFromDetectionArea(Vector3 transformPosition, float f)
        {
            Collider[] colliders = new Collider[3];
            Physics.OverlapSphereNonAlloc(transformPosition, f, colliders, LayerMask.GetMask("Ally", "Player"));
            return colliders.ConvertTo(c =>
                {
                    if (c == null)
                        return new PotentialTarget();
                    return new PotentialTarget(gameObject, c.gameObject);
                }).ToHeap().RemoveFirst()
                .Target;
        }

        protected Vector3 directionToTarget => (currentTarget.transform.position - transform.position).normalized;


        public enum BehaviourType
        {
            Weapon,
            Pathfinding,
            Death
        }

        public void SetBehaviour(BehaviourType type, Action callback)
        {
            switch (type)
            {
                case BehaviourType.Weapon:
                    ONOverridingWeaponBehaviour = callback;
                    break;
                case BehaviourType.Pathfinding:
                    ONOverridingPathfinding = callback;
                    break;
                case BehaviourType.Death:
                    ONOverridingDeathEvent = callback;
                    break;
            }
        }

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            WeaponController = GetComponent<WeaponController>();
            WeaponController.SetDesiredTarget(typeof(PlayerController));
            HealthModifier = GetComponent<HealthModifier>();

            ONOverridingPathfinding = DefaultPathfind;
            ONOverridingWeaponBehaviour = () => BasicWeaponBehaviour(currentTarget, gameObject.transform, attackRange);
        }


        private void Update()
        {
            ONOverridingWeaponBehaviour?.Invoke();
            ONOverridingPathfinding?.Invoke();
        }

        private void FixedUpdate()
        {
            Rigidbody.velocity = currentDirection * Time.fixedDeltaTime;
        }


        protected abstract void DefaultPathfind();

        protected virtual void OnDisable()
        {
            if (HealthModifier.IsFlaggedForDeath)
            {
                ONOverridingDeathEvent?.Invoke();
            }

            if (isFlaggedForReset)
            {
                ResetBehaviour(true);
                isFlaggedForReset = false;
                ONOverridingDeathEvent = null;
                ONOverridingPathfinding = DefaultPathfind;
                ONOverridingWeaponBehaviour =
                    () => BasicWeaponBehaviour(currentTarget, gameObject.transform, attackRange);
            }
        }

        public static void BasicWeaponBehaviour(GameObject currentTarget, Transform owner, float attackRange)
        {
            var weaponController = owner.GetComponent<WeaponController>();
            if (currentTarget)
            {
                weaponController.Aim((currentTarget.transform.position - owner.position)
                    .normalized);
                weaponController.Shoot(IsInsideDetectionRange(currentTarget, owner, attackRange));
            }
        }

        public static bool IsInsideDetectionRange(GameObject target, Transform transform, float range)
        {
            float dist = Vector3.Distance(target.transform.position, transform.position);
            return dist < range;
        }


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

        private Coroutine _coroutine;


        public void OnPathFound(Vector3[] waypoints, bool succeded)
        {
            if (succeded && gameObject.activeSelf)
            {
                Path = new Path(waypoints, transform.position, turnDistance, stoppingDst);
                ResetBehaviour();
                _coroutine = StartCoroutine(FollowPath());
            }
        }

        public void ResetBehaviour(bool resetPath = false)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            if (resetPath)
                Path = null;
        }


        private IEnumerator FollowPath()
        {
            bool followingPath = true;
            int pathIndex = 0;

            if (Path.lookPoints.Length > 0)
                rigidBody.MoveRotation(Quaternion.LookRotation((Path.lookPoints[0] - transform.position)));

            while (followingPath)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
                while (Path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == Path.finishLineIndex)
                    {
                        followingPath = false;
                        break;
                    }
                    else
                    {
                        pathIndex++;
                    }
                }

                if (followingPath)
                {
                    if (pathIndex >= Path.slowDownIndex && stoppingDst > 0)
                    {
                        speedPercent =
                            Mathf.Clamp01(Path.turnBoundaries[Path.finishLineIndex].DistanceFromPoint(pos2D) /
                                          stoppingDst);
                        if (speedPercent < 0.01f)
                        {
                            followingPath = false;
                        }
                    }

                    Quaternion targetRotation =
                        Quaternion.LookRotation(Path.lookPoints[pathIndex] - transform.position);
                    transform.rotation =
                        Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    // transform.Translate(Vector3.forward * (Time.deltaTime * speed * speedPercent), Space.Self);
                    currentDirection = (Path.lookPoints[pathIndex] - transform.position).normalized *
                                       (speed * speedPercent);
                }

                yield return null;
            }

            yield return null;
        }
    }
}