using System.Collections;
using UnityEngine;
using Utility;
using Weaponry.Settings.Bullet_Modifier;

namespace Player.HUD.Abilities
{
    [CreateAssetMenu(fileName = "New Weapon Modifier Ability", menuName = "GMTK/Abilities/Create Weapon Modifier Buff", order = 0)]
    public class ApplyWeaponModifier : Ability
    {
        public BulletModifier Modifier;
        public float duration = 3f;
        public override IEnumerator Activate(PlayerController playerController)
        {
            WeaponSettings currentWeapon =
                playerController.WeaponManager.weaponLibrary[playerController.WeaponManager.CurrentWeapon];
            ParticleSystem fx = ObjectPooler.DynamicInstantiate(abilityFX, playerController.transform.parent);
            fx.Play();
            currentWeapon.bulletModifiers.Add(Modifier);
            float cd = 0;
            while (cd < duration)
            {
                cd += Time.deltaTime;
                fx.transform.position = playerController.transform.position;
                yield return new WaitForEndOfFrame();
            }
            fx.Stop();
            fx.gameObject.SetActive(false);
            currentWeapon.bulletModifiers.Remove(Modifier);
        }
    }
}