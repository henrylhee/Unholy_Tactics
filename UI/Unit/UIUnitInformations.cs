using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIUnitInformations : MonoBehaviour
{
    [SerializeField]
    private Transform actionListContent;
    [SerializeField]
    private UIAction uiActionTemplate;
    [SerializeField]
    private UIStats uiStats;

    private List<UIAction> UIactions = new List<UIAction>();

    [HideInInspector]
    public UnityEvent<Action> actionSelected;
    [HideInInspector]
    public UnityEvent<Action> actionHoverStart;
    [HideInInspector]
    public UnityEvent<Action> actionHoverEnd;

    public void SetUp(List<Action> actions, PlayerUnit playerUnit, UnitUI ui, Inputs input)
    {
        foreach (Action action in actions)
        {
            if (action is ExperimentalMove)
            {
                continue;
            }

            UIAction instance = Instantiate(uiActionTemplate);
            instance.SetUp(action, playerUnit, ui, GetInputString(actions.IndexOf(action),input));

            instance.transform.SetParent(actionListContent, true);
            instance.selected.AddListener(OnActionSelected);
            instance.hoverEnd.AddListener(OnHoverEnd);
            instance.hoverStart.AddListener(OnHoverStart);

            UIactions.Add(instance);
        }

        playerUnit.health.HealthChanged.AddListener(uiStats.UpdateHealthCount);
        
        uiStats.UpdateHealthCount(playerUnit.health.CurrentHp, playerUnit.health.GetMaxHealth());
        uiStats.UpdateAutoDeathCount(playerUnit.stats.activeLifeTimeLimit, playerUnit.stats.lifetimeLimit);
    }

    private void OnActionSelected(Action action)
    {
        actionSelected?.Invoke(action);
    }

    private void OnHoverStart(Action action)
    {
        actionHoverStart?.Invoke(action);
    }

    private void OnHoverEnd(Action action)
    {
        actionHoverEnd?.Invoke(action);
    }

    public void HideTooltip()
    {
        foreach (UIAction uiAction in UIactions)
        {
            uiAction.MouseExit();
        }
    }

    public void UpdateStats(PlayerUnit playerUnit)
    {
        foreach (UIAction uiAction in UIactions)
        {
            uiAction.UpdateStats(playerUnit);
        }
        uiStats.UpdateHealthCount(playerUnit.health.CurrentHp, playerUnit.health.GetMaxHealth());
        uiStats.UpdateAutoDeathCount(playerUnit.stats.activeLifeTimeLimit, playerUnit.stats.lifetimeLimit);
    }

    public void EnableUIActions()
    {
        foreach (UIAction uiAction in UIactions)
        {
            uiAction.Enable();
        }
    }

    public void DisableUIActions()
    {
        foreach (UIAction uiAction in UIactions)
        {
            uiAction.Disable();
        }
    }

    public void DisableUIActions(Action action)
    {
        foreach (UIAction uiAction in UIactions)
        {
            if (uiAction.IsForAction(action))
            {
                uiAction.Disable();
            }
        }
    }

    public void UpdateVisibleUsability(Party party, Unit caster)
    {
        foreach(UIAction uIAction in UIactions)
        {
    
            uIAction.UpdateVisibleUsability(party, caster);
        }
    }

    public void FixPosition()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
        //RectTransform rect = GetComponent<RectTransform>();

       // rect.position = new Vector2(Screen.width - rect.rect.width / 2, rect.rect.height / 2);
    }

    private string GetInputString(int index, Inputs input)
    {
        string text = "";
        switch (index)
        {
            case 0:
                text = input.Unit.Abilty_1.bindings[0].path;
                break;
            case 1:
                text = input.Unit.Abilty_2.bindings[0].path;
                break;
            case 2:
                text = input.Unit.Abilty_3.bindings[0].path;
                break;
            case 3:
                text = input.Unit.Abilty_4.bindings[0].path;
                break;
            case 4:
                text = input.Unit.Abilty_5.bindings[0].path;
                break;
            case 5:
                text = input.Unit.Abilty_6.bindings[0].path;
                break;
        }

        return text.Replace("<Keyboard>/","").ToUpper();
    }
}
