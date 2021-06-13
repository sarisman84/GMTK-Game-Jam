using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Player.HUD.Abilities;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public List<Ability> currentAbilities;


    private Coroutine currentAbilityUsed;

    public float currentCd;

    private void Awake()
    {
        currentCd = 1000000f;
    }

    private void Update()
    {
        currentCd += Time.deltaTime;
    }

    public void UseCurrentAbility(PlayerController playerController, int getKeyDown)
    {
        Debug.Log(getKeyDown);
        if (getKeyDown >= currentAbilities.Count || getKeyDown < 0) return;
        if (currentCd >= currentAbilities[getKeyDown].cooldown)
        {
            StartCoroutine(currentAbilities[getKeyDown].Activate(playerController));
            currentCd = 0;
        }
    }

    private void OnDisable()
    {
        foreach (var currentAbility in currentAbilities)
        {
            currentAbility.Reset();
        }
        currentCd = 1000000f;
    }
}