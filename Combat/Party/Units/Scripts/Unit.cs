using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    //Events
    [HideInInspector]
    public UnityEvent<Unit, int> Die;
    [HideInInspector]
    public UnityEvent<Unit> Resolved;
    [HideInInspector]
    public UnityEvent<int> GeneratedMana;
    [HideInInspector]
    public UnityEvent<Buff> OnApplyBuff;
    [HideInInspector]
    public UnityEvent<Buff> OnRemoveBuff;
    [HideInInspector]
    public UnityEvent<Action> performed;

    [Header("General Unit")]
    [SerializeField]
    private TextAsset table;
    [SerializeField]
    private string ImportID;

    public Vector3 startingPosition { get; private set; }

    public Health health { get; private set; } = new Health();
    public UnitStats stats { get; private set; } = new UnitStats();
    protected Buffs buffs { get; private set; } = new Buffs();

    public UnitModel model { get; private set; }
    public UnitVFX unitVFX { get; private set; }
    public Unit3DUI unit3DUI { get; private set; }

    public Inputs inputs { get; private set; }

    protected CinemachineVirtualCamera cam;
    protected NavMeshAgent navMeshAgent;
    protected NavMeshObstacle navMeshObstacle;

    private StatsTableImporter statsImporter = new StatsTableImporter();

    public bool IsMoving() { return navMeshAgent.enabled && navMeshAgent.remainingDistance != 0f; }


    public virtual void Setup(Inputs inputs)
    {
        GetComponents();
        this.inputs = inputs;
        navMeshAgent.updateRotation = false;

        statsImporter.Import(ImportID, table, this);
        stats.SetUp();

        health.SetUp(GetComponentInChildren<UnitSounds>());
        unit3DUI.SetUp(this, health.GetCurrentHealth(), health.GetMaxHealth(),ImportID, stats.activeLifeTimeLimit);

        health.Die.AddListener(OnDeath);

        model.DeathFinished += unitVFX.PlayFallVFX;
        model.Stepped += unitVFX.PlayStepVfx;
    }
     

    public virtual void TakeDamage(int damage, bool crit)
    {
        if(damage <= 0)
        {
            return;
        }
        
        buffs.OnTakeDamage(this, damage);
        damage += buffs.GetDamageModifier(this);
        model.PlayAnimation(UnitAnimation.TAKE_DAMAGE);
        health.TakeDamage(damage);
        unitVFX.DamageTaken(damage, crit);
    }

    public void PreviewTakeDamage(int minDamage, int maxDamage)
    {
        if(maxDamage <= 0)
        {
            return;
        }
        minDamage += buffs.GetDamageModifier(this);
        maxDamage += buffs.GetDamageModifier(this);

        unit3DUI.AdjustPreviewHealthbar(health.MaxHp, health.CurrentHp, minDamage, maxDamage);
    }

    public void HealDamage(int healing)
    {
        if(healing <= 0)
        {
            return;
        }

        buffs.OnHealDamage(this);
        healing += buffs.GetHealModifier(this);
        health.Heal(healing);
        unitVFX.Healed(healing);
    }


    public void AddBuff(Buff buff, bool copy = true)
    {
        buff = buffs.AddBuff(buff, this, copy);
        OnApplyBuff?.Invoke(buff);
    }

    public void RemoveBuff(Buff buff)
    {
        OnRemoveBuff?.Invoke(buff);
        buffs.RemoveBuff(buff, this);
    }
     
    public virtual void Select()
    {
        Combat.ResetFreeCam();
        cam.Priority = 10;

        StartCoroutine(delay());
        IEnumerator delay()
        {
            yield return new WaitForFixedUpdate();
            navMeshAgent.enabled = false;
            navMeshObstacle.enabled = false;
            yield return new WaitForSeconds(0.1f);
            navMeshAgent.enabled = true;
            FinishSelecting();
        }
    }

    protected virtual void FinishSelecting()
    {

    }
     
    public virtual void Deselect()
    {
        cam.Priority = 9;
        model.DeactivateOutline();

        StartCoroutine(delay());
        IEnumerator delay()
        {
            yield return new WaitForFixedUpdate();
            navMeshObstacle.enabled = false;
            navMeshAgent.enabled = false;
            yield return new WaitForSeconds(0.1f);
            navMeshObstacle.enabled = true;
        }
    }
     

    public virtual void TurnStart()
    {
        startingPosition = transform.position;
        buffs.OnTurnStart(this);

        GeneratedMana?.Invoke(stats.autoManaGeneration);
    }

    public virtual void TurnEnd()
    {
        buffs.OnTurnEnd(this);
        if(stats.activeLifeTimeLimit > 0)
        {
            stats.activeLifeTimeLimit--;
            if (stats.activeLifeTimeLimit == 0)
            {
                OnDeath();
            }
        }
    }

     
    public void HoverStart(bool allied)
    {
        if (allied)
        {
            model.ActivateOutlineAlly();
        }
        else
        {
            model.ActivateOutlineEnemy();
        }
    }

    public void HoverEnd()
    {
        model.DeactivateOutline();
    }

    public void Move(Vector3 targetPosition)
    {
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.destination = targetPosition;
        }
    }

    public void LookIntoMovingDirection()
    {
        model.LookIntoDirection(navMeshAgent.velocity.normalized);
    }

    protected virtual void OnFinishResolving(bool defaultAction)
    {
        if (defaultAction)
        {
            return;
        }

        Resolved?.Invoke(this);
    }
    
    protected virtual void OnDeath()
    {   
        buffs.ClearBuffs(this);
        unit3DUI.Clear();
        model.PlayAnimation(UnitAnimation.DIE);

        navMeshObstacle.enabled = false;
        navMeshAgent.enabled = false;

        cam.Priority = int.MinValue;
        GetComponent<Collider>().enabled = false;

        unitVFX.EndFloorVFX();

        StartCoroutine(delay());
        IEnumerator delay()
        {
            yield return new WaitForSeconds(0.1f);
            Die?.Invoke(this, stats.SoulDrop);
        }
    }

    public float GetWidth()
    {
        return GetComponent<CapsuleCollider>().radius;
    }

    public bool CanReach(Vector3 position)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        return navMeshAgent.CalculatePath(position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete;
    }

    public float GetWalkDistance(Vector3 StartPosition, Vector3 EndPosition)
    {
        bool agentState = navMeshAgent.enabled;

        NavMeshPath navMeshPath = new NavMeshPath();
        navMeshAgent.enabled = true;
        navMeshAgent.CalculatePath(StartPosition, navMeshPath);

        float length = 0;

        for (int i = 0; i < navMeshPath.corners.Length; i++)
        {
            if (i == 0)
            {
                length += Vector3.Distance(transform.position, navMeshPath.corners[i]);
            }
            else
            {
                length += Vector3.Distance(navMeshPath.corners[i - 1], navMeshPath.corners[i]);
            }
        }
        navMeshAgent.enabled = agentState;

        return length;
    }

    private void GetComponents()
    {
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        model = GetComponentInChildren<UnitModel>();
        cam = GetComponentInChildren<CinemachineVirtualCamera>();
        unit3DUI = GetComponentInChildren<Unit3DUI>();
        unitVFX = GetComponentInChildren<UnitVFX>();
    }
}