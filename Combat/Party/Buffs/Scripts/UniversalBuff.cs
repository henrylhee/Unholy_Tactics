using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UniversalBuff", menuName = "Buffs/UniversalBuff", order = 2)]
public class UniversalBuff : Buff
{
    [SerializeField]
    private float buffDamage = 0;
    [SerializeField]
    private float buffCritChance = 0;
    [SerializeField]
    private float buffCritDamage = 0;
    [SerializeField]
    private float buffBlockChance = 0;
    [SerializeField]
    private float buffHitChance = 0;
    [SerializeField]
    private float buffArmorFactor = 0;
    [SerializeField]
    private float buffAbilityRange = 0;
    [SerializeField]
    private float buffHeal = 0;
    [SerializeField]
    private int buffLifeTime = 0;
    [SerializeField]
    private float buffMoveRange = 0;
    [SerializeField]
    private float reduceManaCost = 0;
    [SerializeField]
    private float reduceSoulCost = 0;
    [SerializeField]
    private float reduceHealthCost = 0;
    [SerializeField]
    private float reduceCooldown = 0;

    [SerializeField]
    private List<DotEffect> dotEffects;

    public override void OnApply(Unit caster)
    {
        caster.stats.buffDamage += buffDamage;
        caster.stats.buffCritChance += buffCritChance;
        caster.stats.buffCritDamage += buffCritDamage;
        caster.stats.buffBlockChance += buffBlockChance;
        caster.stats.buffHitChance += buffHitChance;
        caster.stats.buffArmorFactor += buffArmorFactor;
        caster.stats.buffAbilityRange += buffAbilityRange;
        caster.stats.buffHeal += buffHeal;
        caster.stats.activeLifeTimeLimit += buffLifeTime;
        caster.stats.buffMoveRange += buffMoveRange;
        base.OnApply(caster);
    }

    public override void OnRemove(Unit caster)
    {
        caster.stats.buffDamage -= buffDamage;
        caster.stats.buffCritChance -= buffCritChance;
        caster.stats.buffCritDamage -= buffCritDamage;
        caster.stats.buffBlockChance -= buffBlockChance;
        caster.stats.buffHitChance -= buffHitChance;
        caster.stats.buffArmorFactor -= buffArmorFactor;
        caster.stats.buffAbilityRange -= buffAbilityRange;
        caster.stats.buffHeal -= buffHeal;
        caster.stats.activeLifeTimeLimit -= buffLifeTime;
        caster.stats.buffMoveRange -= buffMoveRange;
        base.OnRemove(caster);
    }

    public override void OnTurnEnd(Unit caster)
    {
        if (dotEffects.Count > 0)
        {
            foreach (DotEffect dotEffect in dotEffects)
            {
                dotEffect.OnApply(caster);
            }
        }

        base.OnTurnEnd(caster);
    }
}
