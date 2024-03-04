using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PartyUnitIcon : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<Unit> clicked;

    [SerializeField]
    private Image icon;
    [SerializeField]
    private QuickSelection selection;
    [SerializeField]
    private Animator backgroundAnim;

    [SerializeField]
    private UIBuffIcon buffIconTemplate;
    [SerializeField]
    private Transform buffHolder;
    [SerializeField]
    private Image skullTemplate;
    [SerializeField]
    private Transform skullHolder;

    private HealthBar healthbar;
    [HideInInspector]
    public Unit unit;


    private Animator anim;
    [SerializeField]
    private Animator manaText;


    public void SetUp(PlayerUnit unit)
    {
        this.unit = unit;

        anim = GetComponent<Animator>();
        healthbar = GetComponentInChildren<HealthBar>();

        unit.stats.StatsUpdated.AddListener(OnStatsChanged);
        UpdateHealthBar(unit.health.MaxHp, unit.health.MaxHp);

        icon.sprite = unit.GetIcon();
        unit.health.HealthChanged.AddListener(UpdateHealthBar);

        unit.Selected.AddListener(OnUnitSelect);
        unit.DeSelected.AddListener(OnUnitDeselect);
        unit.Resolved.AddListener(OnUnitUsed);
        unit.OnApplyBuff.AddListener(AddBuff);
        unit.OnRemoveBuff.AddListener(RemoveBuff);

        OnStatsChanged(unit.stats);
    }

    public void Remove(PlayerUnit unit)
    {
        if(this.unit == unit)
        {
            Destroy(this.gameObject);
        }
    }

    public void UpdateHealthBar(int value, int maxValue)
    {
        healthbar.UpdateHealth(value, maxValue);
    }

    public void OnClick()
    {
        clicked?.Invoke(unit);
    }

    private void OnUnitSelect(PlayerUnit unit)
    {
       anim.SetBool("Selected", true);
    }

    private void OnUnitDeselect(PlayerUnit unit)
    {
        anim.SetBool("Selected", false);
    }

    public void OnStartTurn()
    {
        if(backgroundAnim == null)
        {
            return;
        }

        ShowGeneratedMana();
        backgroundAnim?.SetBool("Usable", true);
    }

    public void OnUnitUsed(Unit caster)
    {
        backgroundAnim?.SetBool("Usable", false);
    }

    public void UpdateIndex(int index)
    {
        selection.UpdateIndex(index);
    }

    public void AddBuff(Buff buff)
    {
        UIBuffIcon icon = Instantiate(buffIconTemplate);
        icon.SetUp(buff);
        icon.transform.SetParent(buffHolder);
    }

    public void ShowGeneratedMana()
    {
        if(unit.stats.autoManaGeneration <= 0)
        {
            return;
        }
        manaText.GetComponent<TextMeshProUGUI>().text = "+" + unit.stats.autoManaGeneration.ToString();
        manaText.Play("ManaFloat");
    }

    public void RemoveBuff(Buff buff)
    {
        foreach(UIBuffIcon icon in buffHolder.GetComponentsInChildren<UIBuffIcon>())
        {
            if (icon.IsFor(buff))
            {
                icon.ShutDown();
            }
        }
    }

    public void OnStatsChanged(UnitStats stats)
    {
        foreach(Transform t in skullHolder)
        {
            Destroy(t.gameObject);
        }

        for(int i = 0;i < stats.activeLifeTimeLimit; i++)
        {
            Image skull = Instantiate(skullTemplate);
            skull.transform.SetParent(skullHolder, false);
        }
    }
}
