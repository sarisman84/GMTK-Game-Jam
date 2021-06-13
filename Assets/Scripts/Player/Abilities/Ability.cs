using System.Collections;
using UnityEngine;

namespace Player.HUD.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        public float cooldown;
        public ParticleSystem abilityFX;
        public abstract IEnumerator Activate(PlayerController playerController);
    }
}