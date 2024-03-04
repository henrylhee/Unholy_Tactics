using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HoT buff", menuName = "Buffs/Heal over Time", order = 2)]
public class HealOverTime : Buff
{
    [SerializeField]
    private int healPerTurn;

    public override void OnTurnEnd(Unit caster)
    {
        caster.HealDamage(healPerTurn);
        base.OnTurnEnd(caster);
    }
}
