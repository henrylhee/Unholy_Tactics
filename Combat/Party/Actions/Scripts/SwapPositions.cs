using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Swap positions", menuName = "Actions/Swap positions", order = 1)]
public class SwapPositions : Action
{
    [Header("Swap Positions")]
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

    public override void OnAnimationEnd(Unit caster)
    {
        Vector3 casterPosition = caster.transform.position;
        caster.transform.position = GetTargetUnits(caster)[0].transform.position;
        GetTargetUnits(caster)[0].transform.position = casterPosition;

        caster.Move(caster.transform.position);
        GetTargetUnits(caster)[0].Move(GetTargetUnits(caster)[0].transform.position);
        FinishResolving(caster);
        base.OnAnimationEnd(caster);
    }
    #endregion
}
