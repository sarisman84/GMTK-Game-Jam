using System;
using System.Collections;
using UnityEngine;

namespace Player.HUD.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        public Sprite icon;
        public float cooldown;
        public ParticleSystem abilityFX;
        public abstract IEnumerator Activate(PlayerController playerController, Action onAbilityEndEvent);

        protected float CurrentCd = 0;
        private Coroutine _coroutine;
        private bool _hasBeenUsed;
        public bool isInCooldown { private set; get; }
        public float currentCooldown => CurrentCd;
        public bool isBeingUsed => _hasBeenUsed;


        private void OnDisable()
        {
            CurrentCd = 0;
            _coroutine = null;
            _hasBeenUsed = false;
        }

        public Ability UseAbility(AbilityManager manager, PlayerController playerController, bool input)
        {
            CurrentCd += Time.deltaTime;
            if (CurrentCd >= cooldown && input && !_hasBeenUsed)
            {
                if (_coroutine != null)
                    manager.StopCoroutine(_coroutine);
                manager.StartCoroutine(Activate(playerController, () =>
                {
                    CurrentCd = 0;
                    _hasBeenUsed = false;
                }));
                _hasBeenUsed = true;
            }

            isInCooldown = CurrentCd < cooldown;
            return this;
        }

        public virtual void Reset()
        {
            CurrentCd = 9999;
            _hasBeenUsed = false;
        }
    }
}