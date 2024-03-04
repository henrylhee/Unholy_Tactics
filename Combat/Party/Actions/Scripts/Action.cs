using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum CastVFX
{
    NONE,
    MAGIC,
    PoisonCloud
}

public class Action : ScriptableObject
{
    public enum Targets
    {
        ALL,
        ENEMIES,
        ALLIES,
        SELF,
        OtherAllies
    }

    [HideInInspector] public UnityEvent<Action> finishedResolving;

    [Header("Action")] 
    [SerializeField] private ActionSound actionStartSound;
    [SerializeField] private ActionSound actionEndSound;

    [field: SerializeField] public Sprite icon { get; private set; }
    [SerializeField] protected int manaCost = 0;
    [SerializeField] protected int soulCost = 0;
    [SerializeField] protected int healthCost = 0;
    [SerializeField] protected int coolDown;
    [SerializeField] private bool requiresLineOfSight = true;
    [SerializeField] private Targets canHit;
    [SerializeField] private CastVFX castVFX;
    [SerializeField] private CastVFX targetVFX;

    protected float _range;
    
    [Tooltip(
        "Other target within the splashEffectRange of the main target will also be hit as long as they are matching the canHit rule. Will only be applied if bigger 0.")]
    [SerializeField]
    private float splashEffectRange;

    [SerializeField] private GameObject AoESphere;
    private GameObject splashEffectPreview;

    [field: TextArea]
    [field: SerializeField]
    public string description { get; private set; }

    protected bool previewSetuped = false;
    private bool isResolving = false;
    protected bool usedUp = false;

    private List<Unit> cachedHitUnits = null;
    private Vector3 cachedPosition = new Vector3();
    private Unit aITargetUnit = null;
    protected Vector3 aITargetPosition = new Vector3();

    private int coolDownCounter = 0;
    public int CoolDownCounter => coolDownCounter;
    [Tooltip("Should the cooldown be applied from the start? Default: cooldown only after first use.")]
    public bool startOnCooldown = false;

    public bool IsResolving
    {
        private set { isResolving = value; }

        get => isResolving;
    }

    public virtual void SetUp(Unit caster)
    {
        previewSetuped = false;
        isResolving = false;
        aITargetUnit = null;
        aITargetPosition = new Vector3();
        cachedHitUnits = null;
        cachedPosition = new Vector3();

        if (startOnCooldown)
            coolDownCounter = coolDown;
    }

    public virtual void TurnStart()
    {
        usedUp = false;
    }

    public void TurnEnd()
    {
        if (coolDown <= 0 || coolDownCounter <= 0)
            return;
        coolDownCounter--;
    }

    #region Perform

    public bool Perform_(Unit caster)
    {
        if (!CanPerform(caster))
        {
            return false;
        }

        Perform(caster);

        Combat.GetActiveParty().SpendMana(GetManaCost());

        usedUp = true;
        IsResolving = true;
        caster.TakeDamage(healthCost, false);
        caster.unitVFX.PlayActionParticles(castVFX);
        PlayActionSound(caster, actionStartSound);

        #region Disable certain Inputs
        caster.inputs.Unit.Disable();
        caster.inputs.CameraMovement.Disable();
        caster.inputs.Party.Disable();
        caster.inputs.Turn.End.Disable();
        #endregion

        if (coolDown > 0 && coolDownCounter == 0)
            coolDownCounter = coolDown + 1;

        return true;
    }

    protected virtual bool IsLegal(Unit caster)
    {
        throw new NotImplementedException();
    }

    protected virtual bool Perform(Unit caster)
    {
        if (GetTargetUnits(caster) != null && GetTargetUnits(caster).Count > 0)
        {
            caster.model.LookIntoDirection(GetTargetUnits(caster)[0].transform.position - caster.transform.position);
        }
        cachedHitUnits = GetTargetUnits(caster);
        cachedPosition = GetTargetPosition(caster);
        
        return false;
    }

    protected virtual void FinishResolving(Unit caster)
    {
        IsResolving = false;
        cachedHitUnits = null;
        cachedPosition = new Vector3();
        finishedResolving?.Invoke(this);
        PlayActionSound(caster, actionEndSound);

        caster.inputs.Unit.Enable();
        caster.inputs.CameraMovement.Enable();
        caster.inputs.Party.Enable();
        caster.inputs.Turn.End.Enable();

        caster.model.PlayAnimation(UnitAnimation.IDLE);
    }

    public virtual bool ResolveUpdate(Unit caster)
    {
        return true;
    }

    public bool CanPerform(Unit caster)
    {
        if (usedUp)
        {
            Debug.LogWarning("Error: tried to use the same ability twice in the same turn");
            return false;
        }

        if (IsResolving)
        {
            return false;
        }

        if (!caster.health.CanSurviveDamage(healthCost))
        {
            return false;
        }

        if (!Combat.GetActiveParty().CanSpendMana(manaCost))
        {
            return false;
        }

        if (!IsLegal(caster))
        {
            return false;
        }

        return true;
    }

    #endregion

    #region Preview

    public void Preview(PlayerUnit caster)
    {
        if (!previewSetuped)
        {
            SetupPreview(caster);
        }

        UpdatePreview(caster);
    }

    protected virtual void SetupPreview(PlayerUnit caster)
    {
        caster.actionRange.GenerateActionRange(GetRange(caster), new Vector3(caster.transform.position.x, 0, caster.transform.position.z));

        if (splashEffectRange > 0)
        {
            if (AoESphere == null)
            {
                Debug.LogWarning("AoESphere missing!");
                return;
            }

            splashEffectPreview = Instantiate(AoESphere);
            splashEffectPreview.transform.localScale *= splashEffectRange * 2;
        }

        previewSetuped = true;
    }

    protected virtual void UpdatePreview(PlayerUnit caster)
    {
        #region update Action range
        caster.actionRange.UpdateActionRange(caster.transform.position);
        #endregion

        #region show outlines
        foreach (Unit u in FindObjectsOfType<Unit>())
        {
            if (CanHit(u, caster) && u.health.GetCurrentHealth() > 0)
            {
                if (Combat.GetActiveParty().IsInParty(u))
                {
                    u.model.ActivateOutlineValidAlly();
                }
                else
                {
                    u.model.ActivateOutlineValidEnemy();
                }
            }
            else
            {
                u.model.DeactivateOutline();
            }
        }
        #endregion

        #region show attackpath
        Unit mouseTarget = GetMouseTarget(caster);
        if(mouseTarget != null && Vector3.Distance(caster.transform.position,mouseTarget.transform.position) <= GetRange(caster))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Vector3 offset = new Vector3(0, 1);
            if (CanHit(mouseTarget,caster))
            {
                caster.attackPath.RenderPath(caster.transform.position + offset, mouseTarget.transform.position + offset, true);
            }
            else if (Physics.Raycast(caster.transform.position, mouseTarget.transform.position - caster.transform.position, out hit, Mathf.Infinity, LayerMask.GetMask("Obstacles")))
            {
                caster.attackPath.RenderPath(caster.transform.position + offset, new Vector3(hit.point.x,mouseTarget.transform.position.y,hit.point.z) + offset, false);
            }
        }
        else
        {
            caster.attackPath.ClearPath();
        }
        #endregion

        #region show splash area
        if (splashEffectRange > 0)
        {
            if (GetTargetUnits(caster) != null && GetTargetUnits(caster).Count > 0)
            {
                splashEffectPreview.gameObject.SetActive(true);
                splashEffectPreview.transform.position = GetTargetUnits(caster)[0].transform.position;
            }
            else
            {
                splashEffectPreview?.gameObject.SetActive(false);
            }
        }
        #endregion
    }

    public virtual void ShutDownPreview(PlayerUnit caster)
    {
        caster.actionRange.ClearActionRange();
        caster.attackPath.ClearPath();

        foreach (Unit u in FindObjectsOfType<Unit>())
        {
            u.model.DeactivateOutline();
        }

        if(splashEffectPreview != null)
        {
            Destroy(splashEffectPreview.gameObject);
        }
        previewSetuped = false;
    }

    #endregion

    public void UseUp()
    {
        usedUp = true;
    }
    
    protected List<Unit> GetTargetUnits(Unit caster)
    {
        if(cachedHitUnits != null)
        {
            return cachedHitUnits;
        }

        Unit baseTarget = GetBaseTarget();

        if(baseTarget == null || aITargetUnit != null && EventSystem.current.IsPointerOverGameObject())
        {
            return null;
        }

        if(splashEffectRange <= 0)
        {
            return new List<Unit> { baseTarget};
        }
        else
        {
            return GetSplashTargets(baseTarget);
        }


        Unit GetBaseTarget()
        {
            Unit target = GetMouseTarget(caster);

            if (CanHit(target, caster))
            {
                return target;
            }
            return null;
        }

        List<Unit> GetSplashTargets(Unit caster)
        {
            var hitUnits = new List<Unit>() { baseTarget };

            if (splashEffectRange <= 0)
                return hitUnits;

            foreach(Unit u in FindObjectsOfType<Unit>())
            {
                if (CanHit(u, caster) && Vector3.Distance(u.transform.position, baseTarget.transform.position) <= splashEffectRange && u != baseTarget)
                {
                    hitUnits.Add(u);
                }
            }

            return hitUnits;
        }
    }

    private Unit GetMouseTarget(Unit caster)
    {
        if (aITargetUnit != null)
        {
            return aITargetUnit;
        }

        LayerMask mask = GetMask();
       

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, mask))
        {
            Unit unit = hit.transform.GetComponent<Unit>();
            if (unit == null)
            {
                return null;
            }

            return unit;
        }

        return null;
    }


    protected virtual Vector3 GetTargetPosition(Unit caster, bool fromStartPoint = false)
    {
        if(aITargetPosition != new Vector3())
        {
            return aITargetPosition;
        }

        if(cachedPosition != new Vector3())
        {
            return cachedPosition;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(ray.origin, ray.direction, out hit, float.PositiveInfinity, mask))
        {
            var distance = 0f;

            if (fromStartPoint)
            {
                distance = Vector2.Distance(new Vector2(hit.point.x, hit.point.z),
                new Vector2(caster.startingPosition.x, caster.startingPosition.z));
            }
            else
            {
                distance = Vector2.Distance(new Vector2(hit.point.x, hit.point.z),
               new Vector2(caster.transform.position.x, caster.transform.position.z));
            }
            if (distance <= GetRange(caster) && !EventSystem.current.IsPointerOverGameObject())
            {
                return hit.point;
            }
        }

        return new Vector3();
    }

    private bool CanHit(Unit target, Unit caster, Vector3 hitPoint = new Vector3())
    {
        if(target == null)
        {
            return false;
        }

        #region check distance
        if (hitPoint == new Vector3())
        {
            hitPoint = target.transform.position;
        }
        var distance = Vector2.Distance(new Vector2(hitPoint.x, hitPoint.z), new Vector2(caster.transform.position.x, caster.transform.position.z));
        if(distance > GetRange(caster))
        {
            return false;
        }
        #endregion

        #region check line of sight
        if (requiresLineOfSight)
        {
            LayerMask mask = GetMask();
            mask += LayerMask.GetMask("Obstacles");
            RaycastHit hit;

            if (Physics.Raycast(caster.transform.position, target.transform.position - caster.transform.position, out hit, distance, mask))
            {
                if (hit.transform.GetComponent<Unit>() == null)
                {
                    return false;
                }
            }
        }
        #endregion

        #region check belonging

        switch (canHit)
        {
            case Targets.ALL:
                break;
            case Targets.ALLIES:
                if (!Combat.GetActiveParty().IsInParty(target))
                {
                    return false;
                }
                break;
            case Targets.OtherAllies:
                if (!Combat.GetActiveParty().IsInParty(target) || target == caster)
                {
                    return false;
                }
                break;
            case Targets.SELF:
                if(target != caster)
                {
                    return false;
                }
                break;
            case Targets.ENEMIES:
                if (Combat.GetActiveParty().IsInParty(target))
                {
                    return false;
                }
                break;
        }

        #endregion


        return true;
    }

    private void PlayActionSound(Unit caster, ActionSound actionSound_)
    {
        if (actionSound_ == null || actionSound_.audioClip == null)
        {
            return;
        }

        GameObject instance = new GameObject();
        instance.gameObject.name = name + " Sound";
        instance.transform.position = caster.transform.position;

        AudioSource sound = instance.AddComponent<AudioSource>();
        sound.volume = actionSound_.volume;
        sound.clip = actionSound_.audioClip;
        sound.outputAudioMixerGroup = actionSound_.group;
        sound.Play();
        Destroy(instance, 5);
    }

    #region Animation

    public virtual void OnAnimationEnd(Unit caster)
    {

    }

    public virtual void OnAnimationTrigger(Unit caster)
    {
        if (cachedHitUnits != null && cachedHitUnits.Count > 0)
        {
            cachedHitUnits[0].unitVFX.PlayActionParticles(targetVFX);
        }
    }

    #endregion

    #region Get
    public float GetRange(Unit caster)
    {
        return Mathf.Clamp(_range * caster.stats.baseAbilityRange * caster.stats.buffAbilityRange, 0, _range * caster.stats.baseAbilityRange * caster.stats.buffAbilityRange);
    }

    public LayerMask GetMask()
    {
        switch (canHit)
        {
            case Targets.ENEMIES:
                return LayerMask.GetMask("AIUnit");
            case Targets.ALLIES:
                return LayerMask.GetMask("PlayerUnit");
            case Targets.OtherAllies:
                return LayerMask.GetMask("PlayerUnit");
            case Targets.SELF:
                return LayerMask.GetMask("PlayerUnit");
        }

        return new LayerMask();
    }

    public int GetManaCost()
    {
        return manaCost;
    }

    public int GetSoulCost()
    {
        return soulCost;
    }

    public int GetHealthCost()
    {
        return healthCost;
    }

    public int GetCooldown()
    {
        return coolDown;
    }

    public bool isUsedUp()
    {
        return usedUp;
    }
    #endregion

    #region Attack Stats
    public virtual float GetMinDamage(Unit caster)
    {
        return 0;
    }

    public virtual float GetMaxDamage(Unit caster)
    {
        return 0;
    }

    public virtual float GetCritChance(Unit caster)
    {
        return 0;
    }

    public virtual float GetHitChance(Unit caster)
    {
        return 0;
    }

    public virtual float GetHealing(Unit caster)
    {
        return 0;
    }

    public virtual float GetCritDamage(float damage, Unit caster)
    {
        return 0;
    }
    #endregion

    #region AI manipulations
    public void SetTargetUnit(Unit target)
    {
        aITargetUnit = target;
    }

    public void ClearTargetUnit()
    {
        aITargetUnit = null;
    }

    public void SetTargetPosition(Vector3 pos)
    {
        aITargetPosition = pos;
    }

    public void ClearTargetPosition()
    {
        aITargetPosition = new Vector3();
    }

    public virtual void Abort(Unit caster)
    {
        FinishResolving(caster);
    }
    #endregion
}

[System.Serializable]
public class ActionSound
{
    public AudioClip audioClip;
    public AudioMixerGroup group;
    public float volume = 1;
}