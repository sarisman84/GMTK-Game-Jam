using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Player;
using Player.HUD.Abilities;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public List<Ability> currentAbilities;
    public AbilityDisplay Display;

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
        if (GameMaster.singletonAccess.playerHealth)
            if (GameMaster.singletonAccess.playerHealth.IsFlaggedForDeath)
            {
                ManualReset();
            }
    }

    public void AddAbility(Ability abilityToGiveToPlayer)
    {
        if (!abilityToGiveToPlayer) return;


        currentAbilities.Add(abilityToGiveToPlayer);
        if (Display)
            Display.UpdateIcons(currentAbilities);
    }

    public void ManualReset()
    {
        foreach (var currentAbility in currentAbilities)
        {
            currentAbility.Reset();
        }

        if (Display)
            Display.Reset();
        currentAbilities = new List<Ability>();
        currentCd = 1000000f;
    }
}