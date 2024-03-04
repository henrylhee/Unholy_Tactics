using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Party : MonoBehaviour
{
    //Events
    [HideInInspector]
    public UnityEvent<Party> Die;
    [HideInInspector]
    public UnityEvent ManaUse;
    [HideInInspector]
    public UnityEvent ManaGained;
    [HideInInspector]
    public UnityEvent SoulUse;
    [HideInInspector]
    public UnityEvent<Unit> OnUnitRegistered;

    [SerializeField]
    public int maxMana = 10;
    [SerializeField]
    public int manaCount = 0;

    [HideInInspector]
    public bool disableLoosing = false;
    protected bool active = false;

    protected List<Unit> units = new List<Unit>();
    protected Inputs inputs;

    public virtual void SetUp(Inputs inputs)
    {
        this.inputs = inputs;
        foreach(Unit unit in GetComponentsInChildren<Unit>())
        {
            SetUpUnit(unit);
        }
    }

    public virtual void SetUpUnit(Unit toRegister)
    {
        toRegister.Setup(inputs);
        toRegister.TurnStart();
        toRegister.Die.AddListener(OnUnitDie);
        toRegister.GeneratedMana.AddListener(GainMana);
        units.Add(toRegister);

        OnUnitRegistered?.Invoke(toRegister);
    }
  
    #region Party activity
    public virtual void SetActive()
    {
        active = true;
        foreach(Unit unit in units)
        {
            unit.TurnStart();
        }
    }

    public virtual void SetInactive()
    {
        active = false;
        for(int i = 0;i < units.Count;i++)
        {
            units[i].TurnEnd();
        }
    }

    public virtual void ActiveUpdate()
    {
        
    }

    #endregion

    #region Mana ressource Management
    public bool CanSpendMana(int toSpend)
    {
        return (manaCount - toSpend >= 0);
    }

    public virtual void GainMana(int toGain)
    {
        manaCount = Mathf.Clamp(manaCount + toGain, 0, maxMana);
        ManaGained?.Invoke();
    }

    public virtual bool SpendMana(int toSpend)
    {
        toSpend = Mathf.Clamp(toSpend, 0, int.MaxValue);

        if (manaCount - toSpend < 0)
        {
            return false;
        }

        manaCount -= toSpend;

        if (toSpend > 0)
        {
            ManaUse?.Invoke();
        }

        return true;
    }
    #endregion

    protected virtual void OnUnitDie(Unit Unit, int souls)
    {
        units.Remove(Unit);

        if (units.Count == 0 && !disableLoosing)
        {
            Die?.Invoke(this);
        }

    }

    public bool IsDead()
    {
        return units.Count == 0;
    }

    public bool IsInParty(Unit unit)
    {
        return units.Contains(unit);
    }


    public List<Unit> GetUnits()
    {
        return units;
    }
}
