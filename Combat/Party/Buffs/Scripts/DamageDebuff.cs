using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Debuff", menuName = "Buffs/Damage Debuff", order = 2)]
public class DamageDebuff : Buff
{
    [SerializeField]
    private int damageReduction = 0;

    public override void OnApply(Unit caster)
    {
        caster.stats.buffCritChance -= damageReduction;
        base.OnApply(caster);
    }

    public override void OnRemove(Unit caster)
    {
        caster.stats.buffCritChance += damageReduction;
        base.OnRemove(caster);
    }
}
