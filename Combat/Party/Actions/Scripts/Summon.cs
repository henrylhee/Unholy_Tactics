using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Summon Action", menuName = "Actions/Summon", order = 1)]
public class Summon : Action
{
    [Header("Summon")]

    [SerializeField]
    private int range;
    [SerializeField]
    private LayerMask targetRangeLayer;
    
    [SerializeField] private PlayerUnit summon;
    [SerializeField] private UnitSpawnPreview summonPreview;
    private UnitSpawnPreview activePreview;

    private GameObject rangeIndicator;

    #region Perform
    protected override bool IsLegal(Unit caster)
    {
        if(GetTargetPosition(caster) != new Vector3())
        {
            return true;
        }

        return false;
    }

    public override void SetUp(Unit caster)
    {
        base.SetUp(caster);
        _range = range;
    }

    protected override bool Perform(Unit caster)
    {
        caster.model.PlayAnimation(UnitAnimation.SUMMON);
        caster.model.LookIntoDirection(GetTargetPosition(caster) - caster.transform.position);
        base.Perform(caster);
        return true;
    }

    #endregion

    #region Preview
    protected override void SetupPreview(PlayerUnit caster)
    {
        if (activePreview == null)
        {
            activePreview = Instantiate(summonPreview);
            activePreview.Setup();
        }

        base.SetupPreview(caster);
    }
     
    protected override void UpdatePreview(PlayerUnit caster)
    {
        caster.actionRange.UpdateActionRange(caster.transform.position);
        Vector3 spawnTarget = GetTarget();

        if (spawnTarget == Vector3.zero)
        {
            activePreview.gameObject.SetActive(false);
            return;
        }

        activePreview.gameObject.SetActive(true);
        activePreview.transform.position = new Vector3(spawnTarget.x,caster.transform.position.y,spawnTarget.z);
        activePreview.SetValid(IsLegal(caster));
    }

    public override void ShutDownPreview(PlayerUnit caster)
    {
        if(activePreview != null)
        {
            Destroy(activePreview.gameObject);
            activePreview = null;
        }

        base.ShutDownPreview(caster);
    }
    #endregion

    #region Animation

    public override void OnAnimationTrigger(Unit caster)
    {
        base.OnAnimationTrigger(caster);

        PlayerUnit toSummon = Instantiate(summon);
        Party currentParty = Combat.GetActiveParty();

        caster.StartCoroutine(delay());

        IEnumerator delay()
        {
            toSummon.transform.position = GetTargetPosition(caster) + new Vector3(0, caster.transform.position.y - GetTargetPosition(caster).y, 0);

            yield return new WaitForEndOfFrame();
            
            toSummon.transform.SetParent(currentParty.transform);
            toSummon.GetComponentInChildren<UnitCamera>().Set(caster.GetComponentInChildren<UnitCamera>());
            currentParty.SetUpUnit(toSummon);
            toSummon.Select();

            yield return new WaitForSeconds(0.1f);
            ((PlayerUnit)caster).actionRange.ClearActionRange();
            ((PlayerUnit)caster).actionRange.DeactivateMoveRange();

            FinishResolving(caster);
        }
    }

    #endregion

    #region Other
    private Vector3 GetTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out var hit, Mathf.Infinity, LayerMask.GetMask("AbilityArea")))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
    #endregion
}
