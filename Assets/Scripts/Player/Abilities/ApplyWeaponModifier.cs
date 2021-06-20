using System;
using System.Collections;
using UnityEngine;
using Utility;
using Weaponry.Settings.Bullet_Modifier;

namespace Player.HUD.Abilities
{
    [CreateAssetMenu(fileName = "New Weapon Modifier Ability", menuName = "GMTK/Abilities/Create Weapon Modifier Buff",
        order = 0)]
    public class ApplyWeaponModifier : Ability
    {
        public BulletModifier Modifier;
        public float duration = 3f;
        private ParticleSystem fx;

        public override IEnumerator Activate(PlayerController playerController, Action onAbilityEndEvent)
        {
            fx = ObjectPooler.DynamicInstantiate(abilityFX, playerController.transform.parent);
            fx.Play();

            foreach (var weapon in playerController.WeaponManager.weaponLibrary)
            {
                weapon.bulletModifiers.Add(Modifier);
            }


            float cd = 0;
            while (cd < duration)
            {
                cd += Time.deltaTime;
                fx.transform.position = playerController.transform.position;
                yield return new WaitForEndOfFrame();

                playerController.WeaponManager.weaponLibrary.Find(w => !w.bulletModifiers.Contains(Modifier))
                    ?.bulletModifiers.Add(Modifier);
            }

            Reset();
            foreach (var weapon in playerController.WeaponManager.weaponLibrary)
            {
                weapon.bulletModifiers.Remove(Modifier);
            }
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