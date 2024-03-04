using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddBuff", menuName = "Actions/Add Buff", order = 1)]
public class AddBuff : Action
{
    [Header("AddBuff")]
    [SerializeField]
    private Buff buff;
    [SerializeField]
    private float range;

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
        caster.model.PlayAnimation(UnitAnimation.NORMAL_ATTACK);
        base.Perform(caster);
        return true;
    }
    #endregion

    #region Animation

    public override void OnAnimationTrigger(Unit caster)
    {
        foreach (var unit in GetTargetUnits(caster))
        {
            unit.AddBuff(buff);
        }

        base.OnAnimationTrigger(caster);
    }

    public override void OnAnimationEnd(Unit caster)
    {
        FinishResolving(caster);
    }
    #endregion
}
