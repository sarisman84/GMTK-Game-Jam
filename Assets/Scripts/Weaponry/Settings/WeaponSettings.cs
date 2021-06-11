﻿using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "Basic Weapon Settings", menuName = "GMTK/Weapons/Basic", order = 0)]
    public class WeaponSettings : ScriptableObject
    {
        public float fireRate;
        public Bullet bulletPrefab;


        public virtual void OnShoot(Transform barrel)
        {
            Bullet clone = Instantiate(bulletPrefab,
                barrel.transform.position + (barrel.forward.normalized * 3f), barrel.transform.rotation);
            clone.ONFixedUpdateEvent += bullet =>
            {
                bullet.Rigidbody.velocity = bullet.transform.forward * (bullet.speed * 100f * Time.fixedDeltaTime);
            };
            
            clone.ONCollisionEnterEvent += collider => CloneOnONCollisionEnterEvent(collider, clone);
        }

        private void CloneOnONCollisionEnterEvent(Collider obj, Bullet clone)
        {
            Destroy(clone);
        }
    }
}