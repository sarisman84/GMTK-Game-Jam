using System;
using System.Collections;
using System.Collections.Generic;
using Player.HUD.Abilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class AbilityDisplay : MonoBehaviour
{
    public Image iconPrefab;
    private Dictionary<Ability, AbilityIcon> currentAbilityIcons = new();


    public void UpdateIconList(List<Ability> abilities)
    {
        Reset();
        foreach (var currentAbility in abilities)
        {
            Image image = ObjectPooler.DynamicInstantiate(iconPrefab, transform);
            image.sprite = currentAbility.icon;
            if (!currentAbilityIcons.ContainsKey(currentAbility))
                currentAbilityIcons.Add(currentAbility,
                    new AbilityIcon(image, image.transform.GetChild(0).GetComponent<Image>()));
        }
    }

    public void Reset()
    {
        SetActive(false);
    }

    public void SetActive(bool value)
    {
        foreach (var currentAbilityIcon in currentAbilityIcons)
        {
            if (value)
                UpdateIconState(currentAbilityIcon.Key);
            else
                currentAbilityIcon.Value.CooldownElement.fillAmount = 1;

            currentAbilityIcon.Value.Slot.gameObject.SetActive(value);
        }
    }

    public void UpdateIconState(Ability ability)
    {
        if (currentAbilityIcons.ContainsKey(ability))
        {
            currentAbilityIcons[ability].CooldownElement.fillAmount =
                (ability.cooldown - ability.currentCooldown) / ability.cooldown;
            currentAbilityIcons[ability].Slot.color = ability.isBeingUsed ? Color.gray : Color.white;
        }
    }

    struct AbilityIcon
    {
        public Image Slot;
        public Image CooldownElement;

        public AbilityIcon(Image slot, Image cooldownElement)
        {
            Slot = slot;
            CooldownElement = cooldownElement;
        }
    }
}