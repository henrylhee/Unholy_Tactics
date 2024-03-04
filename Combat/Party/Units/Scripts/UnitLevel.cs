using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[System.Serializable]
public class dsada
{
    private UnitStats unitStats;
    private Health health;

    [Tooltip("Set unlocked abilities starting from level 2.")]
    [SerializeField]
    private List<IntList> actionsPerLevel;

    [SerializeField]
    private XpData xpData;

    [Header("Basestats level modifier")]
    [SerializeField]
    private float hpMultiplier;
    [SerializeField]
    private float lifetimeModifier;
    [SerializeField]
    private float baseDamageMultiplier;
    [SerializeField]
    private float baseAbilityRangeMultiplier;
    [SerializeField]
    private float baseCritChanceMultiplier;
    [SerializeField]
    private float baseCritDamageMultiplier;
    [SerializeField]
    private float blockChanceMultiplier;
    [SerializeField]
    private float armorFactorMultiplier;
    [SerializeField]
    private float manaProductionMultiplier;

    public List<int> xpToLevelUp { private set; get; } = new List<int>();
    private int level;


    public void Setup(UnitStats stats, Health health)
    {
        this.unitStats = stats;
        this.health = health;

        xpToLevelUp = xpData.GetXpList();
        UpdateLevel();

        AdjustStatsToLevel();
    }

    private void AdjustStatsToLevel()
    {
        health.MaxHp = Mathf.RoundToInt(health.MaxHp + hpMultiplier * (level - 1) * health.MaxHp);
        unitStats.lifetimeLimit = Mathf.RoundToInt(unitStats.lifetimeLimit + lifetimeModifier * (level - 1) * unitStats.lifetimeLimit);
        unitStats.autoManaGeneration = Mathf.RoundToInt(unitStats.autoManaGeneration + manaProductionMultiplier * (level - 1) * unitStats.autoManaGeneration);
        unitStats.baseAbilityRange = unitStats.baseAbilityRange + baseAbilityRangeMultiplier * (level - 1) * unitStats.baseAbilityRange;
        unitStats.baseDamage = unitStats.baseDamage + baseDamageMultiplier * (level - 1) * unitStats.baseDamage;
        unitStats.baseCritChance = Mathf.Clamp(unitStats.baseCritChance + baseCritChanceMultiplier * (level - 1) * unitStats.baseCritChance, 0f, 100f);
        unitStats.baseCritDamage = unitStats.baseCritDamage + baseCritDamageMultiplier * (level - 1) * unitStats.baseCritDamage;
        unitStats.blockChance = Mathf.Clamp(unitStats.blockChance + blockChanceMultiplier * (level - 1) * unitStats.blockChance, 0f, 100f);
        unitStats.armorFactor = Mathf.Clamp(unitStats.armorFactor + armorFactorMultiplier * (level - 1) * unitStats.armorFactor, 0f, 100f);
    }

    public List<Action> GetAvailableActions()
    {
        List<Action> actions = new List<Action>();
        if (actionsPerLevel == null)
        {
            return null;
        }
        int length;
        if(xpData.GetMaxLevel() > actionsPerLevel.Count + 1)
        {
            length = actionsPerLevel.Count;
        }
        else
        {
            length = xpData.GetMaxLevel();
        }

        for(int i = 0; i < length && i < actionsPerLevel.Count; i++)
        {
            if (actionsPerLevel[i] == null) { continue; }
            if (actionsPerLevel[i] != null)
            {
                for (int j = 0; j < actionsPerLevel[i].actions.Count; j++)
                {
                    actions.Add(actionsPerLevel[i].actions[j]);
                }
            }
        }
        return actions;
    }

    private void UpdateLevel()
    {
        int levelXp = 0;
        int playerXp = SaveState.Instance.data.playerXp;
        for (int i = 0; i < xpData.GetMaxLevel(); i++)
        {
            levelXp += xpToLevelUp[i];
            if (levelXp > playerXp)
            {
                level = i + 1;
                break;
            }
        }
    }
}

[System.Serializable]
public class IntList
{
    public List<Action> actions;
}
