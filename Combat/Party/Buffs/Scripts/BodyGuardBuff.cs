using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyGuardBuff : Buff
{
    private Unit source;

    public void SetUp(Unit source, int duration)
    {
        description = "This unit is protected from damage for _Duration_ turn.";
        this.source = source;
        base.duration = duration; 
    }

    public override void OnTakeDamage(Unit caster, int damage)
    {
        source.TakeDamage(damage, false);
        base.OnTakeDamage(caster, damage);
    }

    public override int TakeDamageModifier(Unit caster)
    {
        return int.MinValue;
    }
}
