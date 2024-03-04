using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIAction : MonoBehaviour
{
    private Action action; 

    [SerializeField]
    private CanvasGroup group;

    [SerializeField]
    private Color unusableColor;

    private Color usableColor;

    [SerializeField]
    private ActionCost manaCost;
    [SerializeField]
    private ActionCost cooldown;

    [SerializeField]
    private ActionTooltip tooltip;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI shortCutText;

    private Image border;

    [HideInInspector]
    public UnityEvent<Action> selected;
    [HideInInspector]
    public UnityEvent<Action> hoverStart;
    [HideInInspector]
    public UnityEvent<Action> hoverEnd;

    private Button button;

    public void SetUp(Action action, PlayerUnit playerUnit, UnitUI ui, string shortcut)
    {
        button = GetComponent<Button>();
        border = GetComponent<Image>();

        this.action = action;
        icon.sprite = action.icon;
        usableColor = icon.color;

        manaCost.SetValue(action.GetManaCost());

        cooldown.SetValue(action.CoolDownCounter);
        
        tooltip.SetUp(action, playerUnit, ui);

        shortCutText.text = shortcut;
        Disable();
    }

    public bool IsForAction(Action action)
    {
        return this.action == action; 
    }

    public void UpdateStats(Unit playerUnit)
    {
        tooltip.UpdateStats(action, playerUnit);
    }

    public void Pressed()
    {
        selected?.Invoke(action);
    }

    public void Disable()
    {
        group.interactable = false;
        button.interactable = false;

        cooldown.SetValue(action.CoolDownCounter);
    }

    public void Enable()
    {
        if (action.isUsedUp() && action is not ExperimentalMove)
        {
            return;
        }

        group.interactable = true;
        cooldown.SetValue(action.CoolDownCounter);
        if (action.CoolDownCounter > 0)
            return;
        button.interactable = true;
    }
    
    public void MouseEnter()
    {
        hoverStart?.Invoke(action);
        tooltip.Show();
    }

    public void MouseExit()
    {
        hoverEnd?.Invoke(action);
        tooltip.Hide();
    }

    public void UpdateVisibleUsability(Party party, Unit caster)
    {
        if (party.CanSpendMana(action.GetManaCost()))
        {
            Enable();
            ShowUsable(party, caster);
            return;
        }
        ShowUnusable(party, caster);
    }

    public void ShowUnusable(Party p, Unit caster)
    {
        Disable();
        tooltip.ShowUnusable(action, p, caster);
    }

    private void ShowUsable(Party p, Unit caster)
    {
        Enable();
        tooltip.ShowUsable(action, p, caster);
    }
}
