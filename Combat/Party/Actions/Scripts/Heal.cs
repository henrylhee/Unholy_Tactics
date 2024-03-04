using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Heal", menuName = "Actions/Heal", order = 1)]
public class Heal : Action
{
    [Header("Heal")]

    [SerializeField]
    private float range;
    [SerializeField]
    private int healing;

    public override void SetUp(Unit caster)
    {
        base.SetUp(caster);
        _range = range;
    }

    #region Perform
    protected override bool IsLegal(Unit caster)
    {
        return GetTargetUnits(caster) != null;
    }


    protected override bool Perform(Unit caster)
    {
        base.Perform(caster);
        caster.model.PlayAnimation(UnitAnimation.NORMAL_ATTACK);
        return true;
    }
    #endregion

    #region Animation
    public override void OnAnimationEnd(Unit caster)
    {
        FinishResolving(caster);
        base.OnAnimationEnd(caster);
    }

    public override void OnAnimationTrigger(Unit caster)
    {
        foreach (var unit in GetTargetUnits(caster))
        {
            unit.HealDamage(healing);
        }
        base.OnAnimationTrigger(caster);
    }
    #endregion
}

