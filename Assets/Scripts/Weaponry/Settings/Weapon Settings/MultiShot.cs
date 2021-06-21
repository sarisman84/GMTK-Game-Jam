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
                BaseEnemy enemy = controller.GetComponent<BaseEnemy>();
                clone.currentTarget = enemy
                    ? enemy.CurrentTarget
                    : BaseEnemy.TargetType.Enemy;
                clone.lifeDuration = bulletLifetime;

                clone.gameObject.layer =
                    controller.gameObject.GetComponent<PlayerController>() || enemy && enemy.isPossessed
                        ? LayerMask.NameToLayer("Bullet/Ally")
                        : LayerMask.NameToLayer("Bullet/Enemy");

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