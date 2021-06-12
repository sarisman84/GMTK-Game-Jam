using System.Collections.Generic;
using General;
using UnityEngine;
using Utility;
using Utility.Attributes;
using Weaponry.Settings.Bullet_Modifier;

namespace Player
{
    public abstract class WeaponSettings : ScriptableObject
    {
        public float bulletSpeed = 5f;
        public float fireRate;
        public Bullet bulletPrefab;

        public Sprite weaponIcon;
        public GameObject weaponModel;

        [Expose] public ImpactEffect ImpactEffect;
        [Expose] public List<BulletModifier> bulletModifiers;

        public abstract void OnShoot(Transform barrel);
    }
}