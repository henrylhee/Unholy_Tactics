using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoT buff", menuName = "Buffs/Damage over Time", order = 2)]
public class DamageOverTime : Buff
{
    [SerializeField]
    private int damagePerTurn;

    public override void OnTurnEnd(Unit caster)
    {
        caster.TakeDamage(damagePerTurn, false);
        base.OnTurnEnd(caster);
    }
}
