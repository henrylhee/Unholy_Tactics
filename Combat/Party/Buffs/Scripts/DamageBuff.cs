using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageBuff", menuName = "Buffs/DamageBuff", order = 2)]
public class DamageBuff : Buff
{
    [SerializeField]
    private int bonusDamage = 0;

    public override void OnApply(Unit caster)
    {
        caster.stats.buffDamage += bonusDamage;
        base.OnApply(caster);
    }

    public override void OnRemove(Unit caster)
    {
        caster.stats.buffDamage -= bonusDamage;
        base.OnRemove(caster);
    }
}
