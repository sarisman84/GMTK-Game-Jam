using System;
using Enemies;
using Player.HUD;
using UnityEngine;
using UnityEngine.Events;

namespace General
{
    public class HealthModifier : MonoBehaviour
    {
        public float maxHealth;
        public UnityEvent<float> onHealthChanged;
        public UnityEvent onDeath;
        private float currentHealth;
        public bool IsFlaggedForDeath { get; private set; }

        private HealthDisplay _display;

        public HealthDisplay display => _display;

        private void Awake()
        {
            _display = GetComponent<HealthDisplay>();
            ResetHealth();
        }


        public void Heal(float amm)
        {
            currentHealth += amm;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            InvokeHealthChangedEvent(onHealthChanged, _display);
        }

        public void TakeDamage(float amm)
        {
            currentHealth -= amm;
            InvokeHealthChangedEvent(onHealthChanged, _display);

            if (currentHealth <= 0)
            {
                IsFlaggedForDeath = true;
                onDeath?.Invoke();
            }
        }

        private void InvokeHealthChangedEvent(UnityEvent<float> unityEvent, HealthDisplay _display)
        {
            if (_display)
                unityEvent?.Invoke(_display.FormatHealth(currentHealth, maxHealth));
            else
                unityEvent?.Invoke(currentHealth);
        }

        public void ResetHealth()
        {
            currentHealth = maxHealth;
            InvokeHealthChangedEvent(onHealthChanged, _display);
        }

        public void DestroyThis()
        {
            if (gameObject.GetComponent<BaseEnemy>() is { } enemy)
            {
                enemy.isFlaggedForReset = true;
            }

            gameObject.SetActive(false);
            ResetHealth();
        }


        private void OnEnable()
        {
            IsFlaggedForDeath = false;
        }
    }
}