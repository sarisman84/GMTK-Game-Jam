using System;
using General;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "New Damage Setting", menuName = "GMTK/Weapons/Impact/Damage", order = 0)]
    public class DamageOnImpact : ImpactEffect
    {
        public float damage;

        public override void OnImpactEffect(Collider collider, Bullet clone, Transform barrelParent)
        {
            if (collider.GetComponent<HealthModifier>() is { } healthModifier &&
                collider.gameObject.layer != LayerMask.NameToLayer("Ally") && (
                    barrelParent.gameObject.layer != LayerMask.NameToLayer("Ally") ||
                    barrelParent.gameObject.layer != LayerMask.NameToLayer("Player")))

            {
                healthModifier.TakeDamage(damage);
            }
        }
    }
}