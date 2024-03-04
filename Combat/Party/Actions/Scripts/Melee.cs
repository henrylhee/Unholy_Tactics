using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAction", menuName = "Actions/Melee Attack", order = 1)]
public class Melee : Attack
{
    #region Perform
    protected override bool IsLegal(Unit caster)
    {
        return GetTargetUnits(caster) != null;
    }


    protected override bool Perform(Unit caster)
    {
        caster.model.PlayAnimation(UnitAnimation.NORMAL_ATTACK);
        base.Perform(caster);
        return true;
    }
    #endregion

    #region Animation
    public override void OnAnimationTrigger(Unit caster)
    {
        base.OnAnimationTrigger(caster);
        
        foreach (var target in GetTargetUnits(caster))
        {
            if (IsAttackSuccessful(caster, target))
            {
                DamageInformation info = CalculateRealDmg(caster, target);
                target.TakeDamage(info.damage, info.crit);
            }
        }
    }
    
    public override void OnAnimationEnd(Unit caster)
    {
        FinishResolving(caster);
    }
    #endregion
}
