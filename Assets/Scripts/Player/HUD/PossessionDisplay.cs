using System.Collections.Generic;
using Enemies;
using TMPro;
using UnityEngine;

namespace Player.HUD
{
    public class PossessionDisplay : MonoBehaviour
    {
        public TMP_Text textDisplay;

        public void UpdatePossesionDisplay(List<BaseEnemy> possessedEnemies)
        {
            if (textDisplay)
                textDisplay.text = $"Possessed Enemies: {possessedEnemies.Count}";
        }


        public void SetActive(bool state)
        {
            textDisplay.gameObject.SetActive(state);
        }
    }
}