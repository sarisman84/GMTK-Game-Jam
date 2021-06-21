using System;
using Enemies;
using Managers;
using UnityEngine;
using Utility;

namespace Player
{
    [CreateAssetMenu(fileName = "New Multishot Settings", menuName = "GMTK/Weapons/Create/Multi-shot", order = 0)]
    public class MultiShot : WeaponSettings
    {
        public int ammOfBullets = 3;
        public float spreadBetweenBullets = 45f;

        public override void OnShoot(Transform barrel, WeaponController controller)
        {
            for (int i = 0; i < ammOfBullets; i++)
            {
                float angle = barrel.transform.eulerAngles.y +
                              (spreadBetweenBullets * (i - Mathf.FloorToInt(ammOfBullets / 2f)));
                Bullet clone = ObjectPooler.DynamicInstantiate(bulletPrefab,
                    barrel.transform.position + (barrel.forward.normalized * 3f),
                    Quaternion.Euler(0, angle, 0));
                clone.currentTarget = controller.GetComponent<BaseEnemy>() is { } enemy
                    ? enemy.CurrentTarget
                    : BaseEnemy.TargetType.Enemy;
                clone.lifeDuration = bulletLifetime;

                foreach (var bulletModifier in bulletModifiers)
                {
                    bulletModifier.ModifyBullet(clone, this);
                }

                clone.ONFixedUpdateEvent += bullet =>
                {
                    bullet.Rigidbody.velocity =
                        bullet.transform.forward * (bulletSpeed * 100f * Time.fixedDeltaTime);
                };

                clone.ONCollisionEnterEvent += collider =>
                {
                    ImpactEffect.OnImpactEffect(collider, clone, barrel.parent);
                    if (!disableDefaultCollision)
                        clone.gameObject.SetActive(false);
                };
            }
        }
    }
}