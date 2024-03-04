using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "RangeAction", menuName = "Actions/Range Attack", order = 1)]
public class Range : Attack
{
    [Header("Range")]

    [SerializeField]
    private GameObject projectile;

    private float projectileSpeed = 10;

    #region Perform
    protected override bool IsLegal(Unit caster)
    {
        return GetTargetUnits(caster) != null && GetTargetUnits(caster).Count > 0;
    }

    protected override bool Perform(Unit caster)
    {
        caster.model.PlayAnimation(UnitAnimation.NORMAL_ATTACK);
        base.Perform(caster);
        return true;
    }
    #endregion

    #region Preview
    protected override void SetupPreview(PlayerUnit caster)
    {
        caster.actionRange.GenerateActionRange(GetRange(caster), new Vector3(caster.transform.position.x, 0, caster.transform.position.z));
        base.SetupPreview(caster);
    }

    protected override void UpdatePreview(PlayerUnit caster)
    {
        base.UpdatePreview(caster);
        caster.actionRange.UpdateActionRange(caster.transform.position);
    }
    #endregion

    #region Animation
    
    public override void OnAnimationTrigger(Unit caster)
    {
        base.OnAnimationTrigger(caster);
        if(projectile != null)
        {
            caster.StartCoroutine(delay());
        }
        else
        {
            foreach (var unit in GetTargetUnits(caster))
            {
                DealDamage(caster, unit);
            }

            FinishResolving(caster);
        }

        IEnumerator delay()
        {
            float timer = 0;
            float duration = Vector3.Distance(caster.transform.position, GetTargetUnits(caster)[0].transform.position) / projectileSpeed;

            GameObject summon = Instantiate(projectile);
            summon.transform.position = caster.transform.position;

            #region move Projectile to target position
            while (timer < duration)
            {
                summon.transform.position = Vector3.Lerp(caster.transform.position, GetTargetUnits(caster)[0].transform.position, timer / duration);
                summon.transform.LookAt(GetTargetUnits(caster)[0].transform.position);
                timer += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }
            #endregion

            if (summon.GetComponentInChildren<MeshRenderer>() != null) 
            { 
                summon.GetComponentInChildren<MeshRenderer>().enabled = false;
            }

            foreach (var unit in GetTargetUnits(caster))
            {
                DealDamage(caster, unit);
            }


            FinishResolving(caster);
            if(summon.GetComponentInChildren<VisualEffect>() != null)
            {
                summon.GetComponentInChildren<VisualEffect>().enabled = false;
            }
            yield return new WaitForSeconds(1);

            summon.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().Priority = 0;
            Destroy(summon.gameObject,1f);
        }
    }
    #endregion

    private void DealDamage(Unit caster, Unit target)
    {
        if (IsAttackSuccessful(caster, target))
        {
            DamageInformation info = CalculateRealDmg(caster, target);
            target.TakeDamage(info.damage, info.crit);
        }
    }
}
