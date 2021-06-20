using System;
using System.Collections;
using General;
using UnityEngine;
using Utility;

namespace Player.HUD.Abilities
{
    [CreateAssetMenu(fileName = "New Player Buff Ability ", menuName = "GMTK/Abilities/Create Self-Buff", order = 0)]
    public class PlayerModifier : Ability
    {
        public float buffDuration;
        public float damageBuff, movementSpeedBuff;
        [Space] public float instantHealAmm;
        private ParticleSystem fx;

        public override IEnumerator Activate(PlayerController playerController, Action onAbilityEndEvent)
        {
            if (!playerController) yield break;


            float oldMaxMovementSpeed = playerController.maxMovementSpeed;
            float oldaccelerationSpeed = playerController.accelerationSpeed;
            float oldDamage = 0;

            playerController.currentMaxMovementSpeed += movementSpeedBuff;
            playerController.accelerationSpeed += movementSpeedBuff;
            playerController.HealthManager.Heal(instantHealAmm);

            DamageOnImpact damageOnImpact =
                (DamageOnImpact) playerController.WeaponManager
                    .weaponLibrary[playerController.WeaponManager.CurrentWeapon].ImpactEffect;
            if (damageOnImpact != null)
            {
                oldDamage = damageOnImpact.damage;
                damageOnImpact.damage += damageBuff;
            }

            fx =
                ObjectPooler.DynamicInstantiate(abilityFX, playerController.transform.parent);
            fx.Play();
            float cd = 0;
            while (cd < buffDuration)
            {
                if (fx)
                    fx.transform.position = playerController.transform.position;
                cd += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (damageOnImpact != null)
            {
                damageOnImpact.damage = oldDamage;
            }

            playerController.currentMaxMovementSpeed = oldMaxMovementSpeed;
            playerController.currentAccelerationSpeed = oldaccelerationSpeed;

            Reset();
            onAbilityEndEvent?.Invoke();
        }

        public override void Reset()
        {
            if (fx)
            {
                fx.Stop();
                fx.gameObject.SetActive(false);
                fx = null;
            }

            base.Reset();
        }
    }
}