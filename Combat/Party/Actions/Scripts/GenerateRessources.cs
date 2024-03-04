using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerateRessources", menuName = "Actions/Generate Ressources", order = 1)]
public class GenerateRessources : Action
{
    [Header("GenerateRessources")]
    public int mana;

    public override void SetUp(Unit caster)
    {
        _range = Mathf.Infinity;
        base.SetUp(caster);
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
        base.OnAnimationTrigger(caster);

        Party p = Combat.GetActiveParty();
        p.GainMana(mana);
    }

    public override void OnAnimationEnd(Unit caster)
    {
        FinishResolving(caster);
    }
    #endregion
}
