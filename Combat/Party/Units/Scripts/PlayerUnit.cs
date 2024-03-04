using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerUnit : Unit
{
    //Events
    [HideInInspector]
    public UnityEvent<PlayerUnit> Selected;
    [HideInInspector]
    public UnityEvent<PlayerUnit> DeSelected;
    [HideInInspector]
    public UnityEvent<Unit> AnyResolved;
    [HideInInspector]
    public UnityEvent<Action, Unit> abilitySelected;

    [Header("Player Unit")]
    [SerializeField]
    public UnitType unitType;
    [SerializeField]
    private Sprite icon;
    [SerializeField]
    private PlayerActions actions;

    public ActionRange actionRange { get; private set; }
    public AttackPath attackPath { get; private set; }
    public Arrow arrow { get; private set; }

    public UnitUI unitUI;
    private UnitCamera unitCamera;

    public bool IsResolving() { return actions.IsResolving(); }
    
    public override void Setup(Inputs inputs)
    {
        if(Unlocks.instance == null)
        {
            Debug.LogWarning("Error: Tried to start a level outside of the level selection");
        }

        base.Setup(inputs);

        GetReferences();
        
        unitUI.SetUp(GetIcon());
        actions.Setup(unitUI, this, unitType, inputs);
        actions.FinishedResolving.AddListener(OnFinishResolving);
        actions.abilitySelected.AddListener(OnAbilitySelected);
    }

    public override void TakeDamage(int damage, bool crit)
    {
        base.TakeDamage(damage, crit);
        if (damage > 0)
            unitVFX.PlayFriendlyBlood();
    }

    public void SelectedUpdate()
    {
        actions.UpdatePreview();

        if (actions.IsResolving())
        {
            actions.ResolveUpdate();
            return;
        }

        if (inputs.Unit.MousePerform.WasPerformedThisFrame())
        {
            if (actions.CanPerform())
            {
                PerformSelectedAction();
                return;
            }
        }
        else if (inputs.Unit.MouseMove.WasPerformedThisFrame())
        {
            if (actions.CanPerformMovement())
            {
                PerformMoveAction();
                return;
            }
        }
    }


    public override void Select()
    {
        //Debug.Log("select unit");
        Selected?.Invoke(this);

        base.Select();
        unitUI.Show();
    }

    protected override void FinishSelecting()
    {
        actionRange.ClearMoveRange();
        actionRange.GenerateMoveRange(actions.GetMoveRange(this),startingPosition);
        base.FinishSelecting();
    }

    public override void Deselect()
    {
        //Debug.Log("deselct unit");
        base.Deselect();
        unitUI.Hide();
        arrow.ClearPath();
        attackPath.ClearPath();
        actions.DeSelectAction();
        actionRange.ClearMoveRange();
        actionRange.ClearActionRange();

        DeSelected?.Invoke(this);
    }

    public override void TurnStart()
    {
        base.TurnStart();
        actions.TurnStart();
        unitUI.EnableUIActions();
    }

    public override void TurnEnd()
    {
        base.TurnEnd();
        actions.TurnEnd();
    }

    public void OnRessourceUpdated(Party party)
    {
        unitUI.UpdateVisibleUsability(party, this);
    }

    public void OnStatsUpdated(UnitStats stats)
    {
        unitUI.UpdateStats(this);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        actions.ShutDownPreview();
    }

    protected override void OnFinishResolving(bool defaultAction)
    {
        AnyResolved?.Invoke(this);
        Combat.EnableCombatCamera();
        if (defaultAction)
        {
            unitUI.EnableUIActions();
            return;
        }
        base.OnFinishResolving(defaultAction);
    }

    private void OnAbilitySelected(Action action)
    {
        abilitySelected?.Invoke(action, this);
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    private void GetReferences()
    {
        unitCamera = GetComponentInChildren<UnitCamera>();
        unitUI = GetComponentInChildren<UnitUI>();
        stats.StatsUpdated.AddListener(OnStatsUpdated);
        actionRange = GetComponentInChildren<ActionRange>();
        attackPath = GetComponentInChildren<AttackPath>();
        arrow = GetComponentInChildren<Arrow>();
    }

    private void PerformSelectedAction()
    {
        attackPath.ClearPath();
        actions.PerformSelectedAction();
        unitUI.DisableUIAction();
        Combat.DisableCombatCamera();
    }

    private void PerformMoveAction()
    {
        actions.PerformMovementAction();
        unitUI.DisableUIAction();
        Combat.DisableCombatCamera();
    }

    public float GetMovementRange()
    {
        return actions.GetMoveRange(this);
    }

    public bool WithinMovementRange(Vector3 position)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        navMeshAgent.enabled = true;

        if (!navMeshAgent.isOnNavMesh)
        {
            return false;
        }

        navMeshAgent.CalculatePath(position, navMeshPath);

        foreach(Vector3 corner in navMeshPath.corners)
        {
            if(Vector3.Distance(startingPosition,corner) > actions.GetMoveRange(this))
            {
                return false;
            }
        }

        return true;
    }
}
