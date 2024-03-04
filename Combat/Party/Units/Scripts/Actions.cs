using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Actions
{
    [HideInInspector]
    public UnityEvent<bool> FinishedResolving;

    [SerializeField]
    protected List<Action> availableActions;
    [SerializeField]
    protected ExperimentalMove moveAction;
    
    protected Unit caster;

    protected virtual void Setup(Unit owner)
    {
        if (moveAction == null)
        {
            Debug.LogError("Critical Error: default action must be set.");
            return;
        }

        caster = owner;
        CloneAvailableActions();
        CloneMoveAction();
    }

    private void CloneAvailableActions()
    {
        List<Action> temporaryActions = new List<Action>();
        foreach (Action action in availableActions)
        {
            if(action == null)
            {
                continue;
            }

            Action copy = GameObject.Instantiate(action);
            copy.name = copy.name.Replace("(Clone)", "");
            copy.SetUp(caster);

            temporaryActions.Add(copy);
        }

        availableActions = temporaryActions;
    }

    private void CloneMoveAction()
    {
        moveAction = GameObject.Instantiate(moveAction);
        moveAction.name = moveAction.name.Replace("(Clone)", "");
        moveAction.SetUp(caster);
        availableActions.Add(moveAction);
    }

    public virtual void TurnStart()
    {
        foreach (var action in availableActions)
        {
            action.TurnStart();
        }
    }
    
    public void TurnEnd()
    {
        foreach (var action in availableActions)
        {
            action.TurnEnd();
        }
    }
}
