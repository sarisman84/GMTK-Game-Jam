using System.Collections;
using UnityEngine;

namespace Player.HUD.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        public Sprite icon;
        public float cooldown;
        public ParticleSystem abilityFX;
        public abstract IEnumerator Activate(PlayerController playerController);
        public abstract void Reset();
    }
}