using Enemies;
using Managers;
using UnityEngine;
using Utility;

namespace Player
{
    [CreateAssetMenu(fileName = "New Single Shot", menuName = "GMTK/Weapons/Create/Single Shot", order = 0)]
    public class SingleShot : WeaponSettings
    {
        public override void OnShoot(Transform barrel, WeaponController controller)
        {
            Bullet clone = ObjectPooler.DynamicInstantiate(bulletPrefab,
                barrel.transform.position + (barrel.forward.normalized * 3f), barrel.transform.rotation);
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
                bullet.Rigidbody.velocity = bullet.transform.forward * (bulletSpeed * 100f * Time.fixedDeltaTime);
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