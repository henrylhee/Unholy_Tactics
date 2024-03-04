using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestBuff", menuName = "Buffs/Test", order = 2)]
public class TestBuff : Buff
{
    public override void OnTurnEnd(Unit caster)
    {
        Debug.Log("Turn ended");
        base.OnTurnEnd(caster);
    }

    public override void OnTurnStart(Unit caster)
    {
        Debug.Log("Turn started");
        base.OnTurnStart(caster);
    }

    public override int TakeDamageModifier(Unit caster)
    {
        Debug.Log("damage modified");
        return base.TakeDamageModifier(caster);
    }

    public override int HealDamageModifier(Unit caster)
    {
        Debug.Log("heal modified");
        return base.HealDamageModifier(caster);
    }

    public override void OnApply(Unit caster)
    {
        Debug.Log("Applied");
        base.OnApply(caster);
    }

    public override void OnRemove(Unit caster)
    {
        Debug.Log("removed");
        base.OnRemove(caster);
    }

    public override void OnHealDamage(Unit caster)
    {
        Debug.Log("Healed");
        base.OnHealDamage(caster);
    }

    public override void OnTakeDamage(Unit caster, int damage)
    {
        Debug.Log("damaged");
        base.OnTakeDamage(caster, damage);
    }
}
