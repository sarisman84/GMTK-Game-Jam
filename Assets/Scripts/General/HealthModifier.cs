using System;
using Enemies;
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


        public void Heal(float amm)
        {
            currentHealth += amm;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            onHealthChanged?.Invoke(currentHealth);
        }

        public void TakeDamage(float amm)
        {
            currentHealth -= amm;
            onHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                IsFlaggedForDeath = true;
                onDeath?.Invoke();
            }
        }

        public void ResetHealth()
        {
            currentHealth = maxHealth;
        }

        public void DestroyThis()
        {
            if (gameObject.GetComponent<BaseEnemy>() is { } enemy)
            {
                enemy.IsFlaggedForReset = true;
            }

            gameObject.SetActive(false);
            ResetHealth();
        }

        private void Awake()
        {
            ResetHealth();
        }

        private void OnEnable()
        {
            IsFlaggedForDeath = false;
        }
    }
}