using System.Collections.Generic;
using Enemies.Testing;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class PossessionManager : MonoBehaviour
    {
        public Bullet bulletPrefab;
        public Transform barrel;
        public float cooldown = 3f;
        public float minionMinSpace = 2f;


        public List<GameObject> possessedEntities;


        private float _currentCooldown;

        public void ShootPossessionShot(bool input)
        {
            _currentCooldown += Time.deltaTime;

            if (input && _currentCooldown >= cooldown)
            {
                var transform1 = barrel.transform;

                Bullet clone = Instantiate(bulletPrefab,
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
                !possessedEntities.Contains(obj.gameObject) && obj.GetComponent<TestingDummy>() is { } testingDummy)
            {
                possessedEntities.Add(obj.gameObject);
                testingDummy.onOverridingFixedUpdate += FollowPossessor;
                testingDummy.onOverridingWeaponBehaviour += Woolooloo;
                Destroy(bullet);
            }

            Destroy(bullet);
        }

        private void Woolooloo(WeaponController weaponController, Transform owner)
        {
            TestingDummy closestDummy =
                GameMaster.SingletonAccess.GetNearestObjectOfType<TestingDummy>(owner.gameObject, 15f,
                    LayerMask.GetMask("Enemy"), possessedEntities);
            if (closestDummy)
            {
                weaponController.Aim((closestDummy.transform.position - owner.position).normalized);
                weaponController.Shoot(TestingDummy.IsInsideDetectionRange(closestDummy.gameObject, owner, 15f));
            }
        }

        private void FollowPossessor(Rigidbody obj)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);

            if (dist > minionMinSpace)
                obj.velocity = (transform.position - obj.transform.position).normalized * (500f * Time.deltaTime);
            else
                obj.velocity = Vector3.zero;
        }
    }
}