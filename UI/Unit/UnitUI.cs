using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<Action> actionSelected;
    [HideInInspector]
    public UnityEvent<Action> actionHoverStart;
    [HideInInspector]
    public UnityEvent<Action> actionHoverEnd;

    [SerializeField]
    private UIUnitInformations unitInfos;

    [SerializeField]
    private Image unitIcon;

    private Animator anim;


    public void SetUp(Sprite icon)
    {
        anim = GetComponent<Animator>();
        unitIcon.sprite = icon;
    }

    public void SpawnActions(List<Action> actions, PlayerUnit playerUnit, Inputs input)
    {
        unitInfos.SetUp(actions, playerUnit, this, input);
        unitInfos.actionHoverStart.AddListener(OnHoverStart);
        unitInfos.actionHoverEnd.AddListener(OnHoverEnd);
        unitInfos.actionSelected.AddListener(OnActionSelected);
    }

    public void SpawnDefaultAction(Action action, PlayerUnit playerUnit)
    {
        return;
        /*UIAction instance = Instantiate(uiActionTemplate);
        instance.SetUp(action, playerUnit);

        instance.transform.SetParent(defaultActionHolder, true);
        instance.selected.AddListener(OnActionSelected);
        instance.hoverEnd.AddListener(OnHoverEnd);
        instance.hoverStart.AddListener(OnHoverStart);

        UIactions.Add(instance);
        */
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
        unitInfos.HideTooltip();
    }

    public void Hide()
    {
        unitInfos.HideTooltip();
        anim.Play("UnitUIOutroAnimation");
    }

    public void Show()
    {
        anim.Play("UnitUIIntroAnimation");
        StartCoroutine(delay());
        IEnumerator delay()
        {
            float timer = 0;

            while(timer < 1)
            {
                unitInfos.FixPosition();
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            unitInfos.FixPosition();
        }
    }
    
    public void UpdateStats(PlayerUnit playerUnit)
    {
        unitInfos.UpdateStats(playerUnit);
    }

    public void UpdateVisibleUsability(Party party, Unit caster)
    {
        unitInfos.UpdateVisibleUsability(party, caster);
    }

    public void EnableUIActions()
    { 
        unitInfos.EnableUIActions();
    }

    public void DisableUIAction()
    {
        unitInfos.DisableUIActions();
    }

    public void DisableUIAction(Action action)
    {
        unitInfos.DisableUIActions(action);
    }
}
