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
    private List<Image> currentAbilityIcons = new();


    public void UpdateIcons(List<Ability> abilities)
    {
        Reset();
        foreach (var currentAbility in abilities)
        {
            Image image = ObjectPooler.DynamicInstantiate(iconPrefab, transform);
            image.sprite = currentAbility.icon;
            currentAbilityIcons.Add(image);
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
            currentAbilityIcon.gameObject.SetActive(value);
        }
    }
}