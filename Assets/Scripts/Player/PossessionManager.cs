using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PossessionManager : MonoBehaviour
    {
        public Bullet bulletPrefab;
        public Transform barrel;
        public float cooldown = 3f;


        public List<GameObject> possesedEntities;


        private float _currentCooldown;

        public void ShootPosessionShot(bool input)
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
            if (obj.GetComponent<Bullet>() == null && obj.GetComponent<PlayerController>() == null && !possesedEntities.Contains(obj.gameObject))
            {
                possesedEntities.Add(obj.gameObject);
                Destroy(bullet);
            }

            Destroy(bullet);
        }
    }
}