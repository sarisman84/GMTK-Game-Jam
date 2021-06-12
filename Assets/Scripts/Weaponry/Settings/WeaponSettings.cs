using General;
using UnityEngine;
using Utility;

namespace Player
{
    [CreateAssetMenu(fileName = "Basic Weapon Settings", menuName = "GMTK/Weapons/Basic", order = 0)]
    public class WeaponSettings : ScriptableObject
    {
        public float damage = 1f;
        public float fireRate;
        public Bullet bulletPrefab;

        public Sprite weaponIcon;
        public GameObject weaponModel;


        public virtual void OnShoot(Transform barrel)
        {
            Bullet clone = ObjectPooler.DynamicInstantiate(bulletPrefab,
                barrel.transform.position + (barrel.forward.normalized * 3f), barrel.transform.rotation);
            clone.ONFixedUpdateEvent += bullet =>
            {
                bullet.Rigidbody.velocity = bullet.transform.forward * (bullet.speed * 100f * Time.fixedDeltaTime);
            };

            clone.ONCollisionEnterEvent += collider => CloneOnONCollisionEnterEvent(collider, clone, barrel.parent);
        }

        private void CloneOnONCollisionEnterEvent(Collider obj, Bullet clone, Transform barrelParent)
        {
            if (obj.GetComponent<HealthModifier>() is { } healthModifier &&
                obj.gameObject.layer != LayerMask.NameToLayer("Ally") && (
                    barrelParent.gameObject.layer != LayerMask.NameToLayer("Ally") ||
                    barrelParent.gameObject.layer != LayerMask.NameToLayer("Player")))

            {
                healthModifier.TakeDamage(damage);
            }

            clone.gameObject.SetActive(false);
        }
    }
}