using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CritChanceBuff", menuName = "Buffs/CritChanceBuff", order = 2)]
public class CritChanceBuff : Buff
{
    [SerializeField]
    private int chanceIncrease = 0;

    public override void OnApply(Unit caster)
    {
        caster.stats.buffCritChance += chanceIncrease;
        base.OnApply(caster);
    }

    public override void OnRemove(Unit caster)
    {
        caster.stats.buffCritChance -= chanceIncrease;
        base.OnRemove(caster);
    }
}