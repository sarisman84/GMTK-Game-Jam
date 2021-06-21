using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Enemies;
using Enemies.AI;
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
                enemy.SetTarget(typeof(BaseEnemy));
                enemy.healthManager.onDeath.AddListener(() => ClearPossession(enemy));
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

        private void OverrideTargeting(BaseEnemy obj)
        {
            BaseEnemy closestDummy =
                GameMaster.singletonAccess.GetNearestObjectOfType(obj.gameObject, 15f,
                    possessedEntities, "Enemy");
            if (closestDummy)
            {
                obj.weaponController.Aim((closestDummy.transform.position - obj.transform.position).normalized);
                obj.weaponController.Shoot(
                    BasicEnemy.IsInsideDetectionRange(closestDummy.gameObject, obj.transform, obj.attackRange));
            }
            else
            {
                obj.weaponController.Aim((transform.position - obj.transform.position).normalized);
            }
        }

        private float _currentRate;


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
                possessedEntity.healthManager.IsFlaggedForDeath = false;
                possessedEntity.gameObject.SetActive(state);
            }
        }
    }
}