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
        }

        private void Awake()
        {
            ResetHealth();
        }
    }
}