using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[System.Serializable]
public class PlayerActions : Actions
{
    [HideInInspector]
    public UnityEvent<Action> abilitySelected = new UnityEvent<Action>();

    private Action selectedAction;
    private Action hoveredAction;

    public virtual void Setup(UnitUI unitUI, PlayerUnit playerUnit, UnitType type, Inputs inputs)
    {
        AddUnlockedActions(type);

        base.Setup(playerUnit);

        ConnectToUI(unitUI,playerUnit,inputs);
        SetUpEvents(playerUnit);

        inputs.Unit.Abilty_1.performed += SelectAction1;
        inputs.Unit.Abilty_2.performed += SelectAction2;
        inputs.Unit.Abilty_3.performed += SelectAction3;
        inputs.Unit.Abilty_4.performed += SelectAction4;
        inputs.Unit.Abilty_5.performed += SelectAction5;
        inputs.Unit.Abilty_6.performed += SelectAction6;
    }
    
    public void UpdatePreview()
    {
        if (selectedAction != null && selectedAction.IsResolving)
        {
            return;
        }

        moveAction?.Preview((PlayerUnit)caster);

        if(hoveredAction != null && hoveredAction != moveAction)
        {
            hoveredAction.Preview((PlayerUnit)caster);
            return;
        }

        if(selectedAction != moveAction)
        {
            selectedAction?.Preview((PlayerUnit)caster);
        }
    }
    
    public bool ResolveUpdate()
    {
        if (moveAction.IsResolving)
        {
            moveAction.ResolveUpdate((PlayerUnit)caster);
            return true;
        }

        if (selectedAction == null || !selectedAction.IsResolving)
            return false;
        
        return selectedAction.ResolveUpdate((PlayerUnit)caster);
    }

    #region ability selection
    protected void SelectAction(Action toSelect)
    {
        if (toSelect.isUsedUp() || !Combat.GetActiveParty().CanSpendMana(toSelect.GetManaCost()) || toSelect.CoolDownCounter > 0)
        {
            return;
        }

        abilitySelected?.Invoke(toSelect);

        if(selectedAction != null)
        {
            DeSelectAction();
        }

        selectedAction = toSelect;

        if(selectedAction == hoveredAction)
        {
            hoveredAction = null;
        }
    }

    protected void SelectAction1(InputAction.CallbackContext context)
    {
        if (availableActions.Count < 1)
            return;
        SelectAction(availableActions[0]);
    }

    protected void SelectAction2(InputAction.CallbackContext context)
    {
        if (availableActions.Count < 2)
            return;
        SelectAction(availableActions[1]);
    }

    protected void SelectAction3(InputAction.CallbackContext context)
    {
        if (availableActions.Count < 3)
            return;
        SelectAction(availableActions[2]);
    }

    protected void SelectAction4(InputAction.CallbackContext context)
    {
        if (availableActions.Count < 4)
            return;
        SelectAction(availableActions[3]);
    }

    protected void SelectAction5(InputAction.CallbackContext context)
    {
        if (availableActions.Count < 5)
            return;
        SelectAction(availableActions[5]);
    }

    protected void SelectAction6(InputAction.CallbackContext context)
    {
        if (availableActions.Count < 6)
            return;
        SelectAction(availableActions[6]);
    }

    public virtual void DeSelectAction()
    {
        if(selectedAction == null || selectedAction == moveAction)
        {
            return;
        }

        selectedAction?.ShutDownPreview((PlayerUnit)caster);
        selectedAction = null;
    }

    #endregion

    #region Hover
    private void HoverStart(Action action)
    {
        if (action == selectedAction || selectedAction != null && selectedAction.IsResolving)
        {
            return;
        }
        hoveredAction = action;
        if (selectedAction != moveAction)
        {
            selectedAction?.ShutDownPreview((PlayerUnit)caster);
        }
    }

    private void HoverEnd(Action action)
    {
        if (action == selectedAction || action == null || action.IsResolving)
        {
            return;
        }
        hoveredAction = null;

        action?.ShutDownPreview((PlayerUnit)caster);
    }
    #endregion


    public void OnFinishResolving(Action action)
    {
        if (action == moveAction)
        {
            FinishedResolving?.Invoke(true);
            return;
        }

        DeSelectAction();

        foreach (Action a in availableActions)
        {
            if (a != moveAction)
            {
                a.UseUp();
            }
        }

        FinishedResolving?.Invoke(false);
    }

    public void ShutDownPreview()
    {
        moveAction?.ShutDownPreview((PlayerUnit)caster);
        selectedAction?.ShutDownPreview((PlayerUnit)caster);
    }


    public void PerformSelectedAction()
    {
        if (selectedAction == null)
        {
            return;
        }
        var result = selectedAction.Perform_((PlayerUnit)caster);
        ShutDownPreview();
    }

    public void PerformMovementAction()
    {
        if (moveAction == null)
        {
            return;
        }
        var result = moveAction.Perform_((PlayerUnit)caster);
    }

    public bool CanPerform()
    {
        return selectedAction != null && selectedAction.CanPerform((PlayerUnit)caster);
    }

    public bool CanPerformMovement()
    {
        return moveAction != null && moveAction.CanPerform((PlayerUnit)caster);
    }

    public bool IsResolving()
    {
        if (moveAction.IsResolving)
        {
            return true;
        }

        if (selectedAction == null)
            return false;

        return selectedAction.IsResolving;
    }


    private void ConnectToUI(UnitUI unitUI, PlayerUnit playerUnit, Inputs input)
    {
        unitUI.SpawnActions(availableActions, playerUnit, input);

        unitUI.actionSelected.AddListener(SelectAction);
        unitUI.actionHoverEnd.AddListener(HoverEnd);
        unitUI.actionHoverStart.AddListener(HoverStart);
    }

    private void AddUnlockedActions(UnitType type)
    {
        if (Unlocks.instance != null)
        {
            foreach (Action action in Unlocks.instance.GetUnlockedActions(type))
            {
                base.availableActions.Add(action);
            }
        }

    }

    private void SetUpEvents(PlayerUnit playerUnit)
    {
        playerUnit.model.AnimationTrigger += () => selectedAction?.OnAnimationTrigger((PlayerUnit)caster);
        playerUnit.model.ActionAnimationEnd += () => selectedAction?.OnAnimationEnd((PlayerUnit)caster);

        foreach (var action in availableActions)
        {
            action.finishedResolving.AddListener(OnFinishResolving);
        }
    }

    public float GetMoveRange(Unit caster)
    {
        return moveAction.GetMovementRange(caster);
    }
}