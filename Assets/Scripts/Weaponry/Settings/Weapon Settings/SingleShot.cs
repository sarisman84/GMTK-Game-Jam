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
            clone.currentTarget = controller.CurTarget;
            foreach (var bulletModifier in bulletModifiers)
            {
                bulletModifier.ModifyBullet(clone);
            }

            clone.ONFixedUpdateEvent += bullet =>
            {
                bullet.Rigidbody.velocity = bullet.transform.forward * (bulletSpeed * 100f * Time.fixedDeltaTime);
            };

            clone.ONCollisionEnterEvent += collider =>
            {
                ImpactEffect.OnImpactEffect(collider, clone, barrel.parent);
                clone.gameObject.SetActive(false);
                GameMaster.singletonAccess.possessor.AdditionToCurrentKillCount = 1;
            };
        }
    }
}