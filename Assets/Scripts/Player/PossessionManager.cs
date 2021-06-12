using System;
using System.Collections.Generic;
using Enemies;
using Enemies.Testing;
using Managers;
using UnityEngine;
using Utility;

namespace Player
{
    public class PossessionManager : MonoBehaviour
    {
        public Bullet bulletPrefab;
        public Transform barrel;
        public float cooldown = 3f;
        public float minionMinSpace = 2f;


        public List<GameObject> possessedEntities;
        public event Action<BaseEnemy> ONPossessionEvent;

        private float _currentCooldown;

        public void ShootPossessionShot(bool input)
        {
            _currentCooldown += Time.deltaTime;

            if (input && _currentCooldown >= cooldown)
            {
                var transform1 = barrel.transform;

                Bullet clone = ObjectPooler.DynamicInstantiate(bulletPrefab,
                    transform1.position + (barrel.forward.normalized * 3f), transform1.rotation);

                clone.speed = 50;
                clone.ONFixedUpdateEvent += bullet =>
                    bullet.Rigidbody.velocity = bullet.transform.forward * (bullet.speed * 100f * Time.fixedDeltaTime);


                clone.ONCollisionEnterEvent += collider1 => OnBulletCollisionEnter(collider1, clone);
                _currentCooldown = 0;
            }
        }

        private void OnBulletCollisionEnter(Collider obj, Bullet bullet)
        {
            if (obj.GetComponent<Bullet>() == null && obj.GetComponent<PlayerController>() == null &&
                !possessedEntities.Contains(obj.gameObject) && obj.GetComponent<BaseEnemy>() is { } enemy)
            {
                possessedEntities.Add(obj.gameObject);
                enemy.ONOverridingFixedUpdate += FollowPossessor;
                enemy.ONOverridingWeaponBehaviour += OverrideTargeting;
                enemy.gameObject.layer = LayerMask.NameToLayer("Ally");
                
                ONPossessionEvent?.Invoke(enemy);
            }

            bullet.gameObject.SetActive(false);
        }

        private void OverrideTargeting(WeaponController weaponController, Transform owner)
        {
            BaseEnemy closestDummy =
                GameMaster.SingletonAccess.GetNearestObjectOfType<BaseEnemy>(owner.gameObject, 15f,
                    LayerMask.GetMask("Enemy"), possessedEntities);
            if (closestDummy)
            {
                weaponController.Aim((closestDummy.transform.position - owner.position).normalized);
                weaponController.Shoot(TestingDummy.IsInsideDetectionRange(closestDummy.gameObject, owner, 15f));
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
    }
}