using System;
using System.Collections.Generic;
using Enemies;
using Enemies.Testing;
using General;
using Managers;
using Player.HUD;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utility;

namespace Player
{
    public class PossessionManager : MonoBehaviour
    {
        public Bullet bulletPrefab;
        public Transform barrel;
        public float minionMinSpace = 2f;
        public float ammOfKillsRequired = 5f;

        public List<BaseEnemy> possessedEntities;
        public event Action<BaseEnemy> ONPossessionEvent;

        private float _currentKillcount;
        private Transform _parentForPossessed;

        public UnityEvent<float> ONKillCountUpdate;
        public UnityEvent ONDisableEvent;
        public UnityEvent ONEnableEvent;


        private PossessionDisplay _display;

        public PossessionDisplay display => _display;


        public float AdditionToCurrentKillCount
        {
            set => _currentKillcount += value;
        }

        private void Awake()
        {
            _display = GetComponent<PossessionDisplay>();
        }

        private void Start()
        {
            _parentForPossessed = new GameObject("Possessions").transform;
        }

        private void Update()
        {
            ONKillCountUpdate?.Invoke(_currentKillcount / 5);
        }

        public void SetUIActive(bool state)
        {
            _display.SetActive(state);
            if (state)
            {
                ONEnableEvent?.Invoke();
                return;
            }

            ONDisableEvent?.Invoke();
        }

        public void ShootPossessionShot(bool input)
        {
            if (input && _currentKillcount >= ammOfKillsRequired)
            {
                var transform1 = barrel.transform;

                Bullet clone = ObjectPooler.DynamicInstantiate(bulletPrefab,
                    transform1.position + (barrel.forward.normalized * (3f + 5f)), transform1.rotation);
                clone.transform.localScale = Vector3.one * 5f;
                clone.ONFixedUpdateEvent += bullet =>
                    bullet.Rigidbody.velocity = bullet.transform.forward * (50 * 100f * Time.fixedDeltaTime);


                clone.ONCollisionEnterEvent += collider1 => OnBulletCollisionEnter(collider1, clone);
                _currentKillcount = 0;
            }
        }

        private void OnBulletCollisionEnter(Collider obj, Bullet bullet)
        {
            if (obj.GetComponent<Bullet>() == null && obj.GetComponent<PlayerController>() == null &&
                obj.GetComponent<BaseEnemy>() is { } enemy &&
                !possessedEntities.Contains(enemy))
            {
                possessedEntities.Add(enemy);
                enemy.WeaponManager.SetDesiredTarget(typeof(BaseEnemy));
                enemy.ONOverridingFixedUpdate += FollowPossessor;
                enemy.ONOverridingWeaponBehaviour += OverrideTargeting;
                enemy.ONOverridingDeathEvent += () => ClearPossession(enemy);
                enemy.gameObject.layer = LayerMask.NameToLayer("Ally");
                enemy.GetComponent<HealthModifier>().ResetHealth();
                ONPossessionEvent?.Invoke(enemy);
                ObjectPooler.RemoveObjectFromPool(enemy.gameObject);
                enemy.transform.parent = null;
                enemy.transform.SetParent(_parentForPossessed);
                display.UpdatePossesionDisplay(possessedEntities);
                display.CreatePossessionTether(enemy);
            }

            bullet.gameObject.SetActive(false);
        }

        private void ClearPossession(BaseEnemy baseEnemy)
        {
            baseEnemy.gameObject.layer = LayerMask.NameToLayer("Enemy");
            possessedEntities.Remove(baseEnemy);
            display.UpdatePossesionDisplay(possessedEntities);
            display.RemoveTether(baseEnemy);
        }

        private void OverrideTargeting(WeaponController weaponController, Transform owner)
        {
            BaseEnemy closestDummy =
                GameMaster.singletonAccess.GetNearestObjectOfType(owner.gameObject, 15f,
                    possessedEntities, "Enemy");
            if (closestDummy)
            {
                weaponController.Aim((closestDummy.transform.position - owner.position).normalized);
                weaponController.Shoot(Enemy.IsInsideDetectionRange(closestDummy.gameObject, owner, 15f));
            }
        }

        private void FollowPossessor(Rigidbody obj)
        {
            if (!obj) return;
            float dist = Vector3.Distance(transform.position, obj.transform.position);

            if (dist > minionMinSpace)
                obj.velocity = (transform.position - obj.transform.position).normalized * (500f * Time.deltaTime);
            else
                obj.velocity = Vector3.zero;
        }

        /// <summary>
        /// Destroys all possessions and resets the possession list.
        /// </summary>
        public void ResetPossessions()
        {
            foreach (var possessedEntity in possessedEntities)
            {
                Destroy(possessedEntity.gameObject);
            }

            Debug.Log("Resetting possessions!");

            possessedEntities = new List<BaseEnemy>();
            display.UpdatePossesionDisplay(possessedEntities);
            display.ResetTether();
        }

        public void TeleportPossessionsToPosition(Vector3 position)
        {
            foreach (var possessedEntity in possessedEntities)
            {
                possessedEntity.transform.position =
                    GameMaster.singletonAccess.GetRandomPositionAroundPoint(position, minionMinSpace);
            }
        }

        public void SetPossessionsActive(bool state)
        {
            foreach (var possessedEntity in possessedEntities)
            {
                possessedEntity.GetComponent<BaseEnemy>().IsFlaggedForReset = false;
                possessedEntity.gameObject.SetActive(state);
            }
        }
    }
}