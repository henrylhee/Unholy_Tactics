using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TooltipCosts : MonoBehaviour
{
    [SerializeField]
    private Image souls;
    [SerializeField]
    private Image mana;
    [SerializeField]
    private Image health;

    [SerializeField]
    private Color defaultColor;
    [SerializeField]
    private Color unusableColor;

    public void SetUp(Action action)
    {
        defaultColor = souls.color;

        souls.gameObject.SetActive(action.GetSoulCost() > 0);
        souls.GetComponentInChildren<TextMeshProUGUI>().text = action.GetSoulCost().ToString();
        mana.gameObject.SetActive(action.GetManaCost() > 0);
        mana.GetComponentInChildren<TextMeshProUGUI>().text = action.GetManaCost().ToString();
        health.gameObject.SetActive(action.GetHealthCost() > 0);
        health.GetComponentInChildren<TextMeshProUGUI>().text = action.GetHealthCost().ToString();
    }

    public void UpdateVisibleUsability(Action action, Party party, Unit caster)
    {
        if(!party.CanSpendMana(action.GetManaCost()))
        {
            mana.color = unusableColor;
        }
        else
        {
            mana.color = defaultColor;
        }

        if(action.GetHealthCost() > caster.health.GetCurrentHealth())
        {
            health.color = unusableColor;
        }
        else
        {
            health.color = defaultColor;
        }
    }
}
