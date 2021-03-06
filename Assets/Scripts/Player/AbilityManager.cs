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
    public AbilityDisplay display;

    private Coroutine _currentAbilityUsed;

    private float _currentCd;

    private void Awake()
    {
        ResetCd();
    }

    private void Update()
    {
        _currentCd += Time.deltaTime;
    }

    public void UseCurrentAbility(PlayerController playerController, int getKeyDown)
    {
        for (var i = 0; i < currentAbilities.Count; i++)
        {
            var ability = currentAbilities[i];
            display.UpdateIconState(ability.UseAbility(this, playerController, i == getKeyDown));
        }
    }

    private void OnDisable()
    {
        if (GameMaster.singletonAccess.playerHealth)
            if (GameMaster.singletonAccess.playerHealth.IsFlaggedForDeath)
            {
                FullReset();
            }
    }

    public void AddAbility(Ability abilityToGiveToPlayer)
    {
        if (!abilityToGiveToPlayer) return;


        currentAbilities.Add(abilityToGiveToPlayer);
        if (display)
            display.UpdateIconList(currentAbilities);
    }

    /// <summary>
    /// Removes all abilities and resets the ability displayer.
    /// </summary>
    public void FullReset()
    {
        foreach (var currentAbility in currentAbilities)
        {
            currentAbility.Reset();
        }

        if (display)
            display.Reset();
        currentAbilities = new List<Ability>();
    }

    public void DisruptAbilities()
    {
    }

    public void ResetCd()
    {
        foreach (var ability in currentAbilities)
        {
            ability.Reset();
        }
    }
}