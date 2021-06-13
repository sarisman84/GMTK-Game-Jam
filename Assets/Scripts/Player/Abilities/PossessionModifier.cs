using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using General;
using UnityEngine;
using Utility;

namespace Player.HUD.Abilities
{
    [CreateAssetMenu(fileName = "New Possession Modifier", menuName = "GMTK/Abilities/Create Possession Buff",
        order = 0)]
    public class PossessionModifier : Ability
    {
        public float buffDuration;
        public float damageBuff, movementSpeedBuff;
        [Space] public float instantHealAmm;

        private List<ParticleSystem> fx;

        public override IEnumerator Activate(PlayerController playerController)
        {
            List<BaseEnemy> currentPossesedEnemies = playerController.PossessionManager.possessedEntities;
            List<float> oldMovementSpeed = new List<float>();
            List<float> oldDamageBuff = new List<float>();
            fx = new List<ParticleSystem>();
            ApplyEffect(currentPossesedEnemies, (enemy, index) =>
            {
                oldMovementSpeed.Add(enemy.MovementSpeed);
                DamageOnImpact damageOnImpact =
                    (DamageOnImpact) enemy.WeaponManager.weaponLibrary[enemy.WeaponManager.CurrentWeapon].ImpactEffect;
                if (damageOnImpact)
                    oldDamageBuff.Add(damageOnImpact.damage);

                enemy.MovementSpeed += movementSpeedBuff;
                enemy.GetComponent<HealthModifier>().Heal(instantHealAmm);
                if (damageOnImpact)
                    damageOnImpact.damage += damageBuff;

                fx.Add(ObjectPooler.DynamicInstantiate(abilityFX, playerController.transform.parent));
                fx[index].Play();
            });


            float cd = 0;
            while (cd < buffDuration)
            {
                cd += Time.deltaTime;
                yield return new WaitForEndOfFrame();
                ApplyEffect(currentPossesedEnemies,
                    (enemy, i) =>
                    {
                        if (i >= fx.Count) return;
                        fx[i].transform.position = enemy.transform.position;
                    });
            }

            ApplyEffect(currentPossesedEnemies, (enemy, index) =>
            {
                enemy.MovementSpeed = oldMovementSpeed[index];
                DamageOnImpact damageOnImpact =
                    (DamageOnImpact) enemy.WeaponManager.weaponLibrary[enemy.WeaponManager.CurrentWeapon].ImpactEffect;

                if (damageOnImpact)
                    damageOnImpact.damage = oldDamageBuff[index];

                fx[index].gameObject.SetActive(false);
            });
        }

        public void ApplyEffect(List<BaseEnemy> targetEnemies, Action<BaseEnemy, int> method)
        {
            for (var i = 0; i < targetEnemies.Count; i++)
            {
                var targetEnemy = targetEnemies[i];
                method?.Invoke(targetEnemy, i);
            }
        }

        public override void Reset()
        {
            foreach (var pFX in fx)
            {
                if (pFX)
                {
                    pFX.Stop();
                    pFX.gameObject.SetActive(false);
                }
            }

            fx = new List<ParticleSystem>();
        }
    }
}