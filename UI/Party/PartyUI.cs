using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI : MonoBehaviour
{
    [SerializeField]
    private PartyUnitIcon iconTemplate;
    [SerializeField]
    private Transform iconHolder;

    [SerializeField]
    private SoulCounter soulText;
    [SerializeField]
    private ManaBar manaBar;
    [SerializeField]
    private QueenInfoPanel queenInfo;

    private List<PartyUnitIcon> icons = new List<PartyUnitIcon>();

    public void AddPartyUnit(PlayerUnit unit)
    {
        if(unit is MainUnit)
        {
            queenInfo.SetUp((MainUnit)unit);
        }

        PartyUnitIcon instance = Instantiate(iconTemplate);
        instance.SetUp(unit);
        instance.transform.SetParent(iconHolder);
        instance.clicked.AddListener(IconUnitSelection);
        icons.Add(instance);

        UpdateIconIndexes();
    }

    public void RemovePartyUnit(PlayerUnit unit)
    {
        PartyUnitIcon toRemove = null;
        foreach(PartyUnitIcon icon in icons)
        {
            if(icon.unit == unit)
            {
                toRemove = icon;
            }

            icon.Remove(unit);
        }

        icons.Remove(toRemove);

        UpdateIconIndexes();
    }

    public void UpdateManaCount(int count, int max)
    {
        manaBar.UpdateValue(count, max);
    } 

    public void UpdateSoulCount(int count) 
    {
        soulText.UpdateCounter(count);
    }

    public void IconUnitSelection(Unit unit)
    {
        unit.Select();
    }

    public void StartTurn()
    {
        for(int i =0;i < icons.Count;i++)
        {
            icons[i].OnStartTurn();
        }

        UpdateIconIndexes();
    }

    private void UpdateIconIndexes()
    {
        int index = 0;
        foreach(PartyUnitIcon icon in icons)
        {
            index++;
            icon.UpdateIndex(index);
        }
    }
}
