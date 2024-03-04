using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BodyGuard", menuName = "Actions/BodyGuard", order = 1)]
public class BodyGuard : Action
{
    [Header("BodyGuard")]
    [SerializeField]
    private float range;
    [SerializeField]
    private int buffDuration;

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
        BodyGuardBuff buff = ScriptableObject.CreateInstance<BodyGuardBuff>();
        buff.SetUp(caster, buffDuration);

        foreach(Unit target in GetTargetUnits(caster))
        {
            target.AddBuff(buff, false);
        }

        base.OnAnimationTrigger(caster);
    }

    public override void OnAnimationEnd(Unit caster)
    {
        FinishResolving(caster);
    }
    #endregion
}
