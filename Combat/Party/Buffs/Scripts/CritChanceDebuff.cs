using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CritChanceDebuff", menuName = "Buffs/CritChanceDebuff", order = 2)]
public class CritChanceDebuff : Buff
{
    [SerializeField]
    private int chanceDecrease = 0;

    public override void OnApply(Unit caster)
    {
        caster.stats.buffCritChance -= chanceDecrease;
        base.OnApply(caster);
    }

    public override void OnRemove(Unit caster)
    {
        caster.stats.buffCritChance += chanceDecrease;
        base.OnRemove(caster);
    }
}