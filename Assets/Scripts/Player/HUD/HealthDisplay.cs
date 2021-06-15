using UnityEngine;
using UnityEngine.UI;

namespace Player.HUD
{
    public class HealthDisplay : MonoBehaviour
    {
        public Scrollbar healthBar;
        public float FormatHealth(float currentHealth, float maxHealth)
        {
            return currentHealth / maxHealth;
        }

        public void SetActive(bool state)
        {
            healthBar.gameObject.SetActive(state);
        }
    }
}