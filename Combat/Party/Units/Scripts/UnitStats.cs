using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnitStats
{
    [HideInInspector]
    public UnityEvent<UnitStats> StatsUpdated = new UnityEvent<UnitStats>();

    //static stats
    [field:SerializeField]
    public int SoulDrop { private set; get; } = 1;

    //dynamic stats
    [SerializeField]
    private int _autoManaGeneration = 1;
    public int autoManaGeneration { set { _autoManaGeneration = value; StatsUpdated?.Invoke(this); } get { return _autoManaGeneration; } }

    [SerializeField]
    private int _lifetimeLimit = 0;
    public int lifetimeLimit { set { _lifetimeLimit = value; StatsUpdated?.Invoke(this); } get { return _lifetimeLimit; } }

    private int _activeLifeTimeLimit = 0;
    public int activeLifeTimeLimit { set { _activeLifeTimeLimit = value; StatsUpdated?.Invoke(this); } get { return _activeLifeTimeLimit; } }

    [SerializeField]
    private float _baseDamage = 1;
    public float baseDamage { set { _baseDamage = value; StatsUpdated?.Invoke(this); } get { return _baseDamage; } }

    [SerializeField]
    private float _baseCritChance = 1;
    public float baseCritChance { set { _baseCritChance = value; StatsUpdated?.Invoke(this);  } get { return _baseCritChance; } }

    [SerializeField]
    private float _baseCritDamage = 1;
    public float baseCritDamage { set { _baseCritDamage = value; StatsUpdated?.Invoke(this); } get { return _baseCritDamage; } }

    [SerializeField]
    private float _baseAbilityRange = 1;
    public float baseAbilityRange { set { _baseAbilityRange = value; StatsUpdated?.Invoke(this); } get { return _baseAbilityRange; } }

    [SerializeField]
    private float _armorFactor = 1;
    public float armorFactor { set { _armorFactor = value; StatsUpdated?.Invoke(this); } get { return _armorFactor; } }

    [SerializeField]
    private float _blockChance = 1;
    public float blockChance { set { _blockChance = value; StatsUpdated?.Invoke(this); } get { return _blockChance; } }

    [SerializeField]
    private float _hitChance = 1;
    public float hitChance { set { _hitChance = value; StatsUpdated?.Invoke(this); } get { return _hitChance; } }


    //members for buffs
    private float _buffDamage = 0;
    public float buffDamage { set { _buffDamage = value; StatsUpdated?.Invoke(this); } get { return _buffDamage; } }

    private float _buffCritChance = 1;
    public float buffCritChance { set { _buffCritChance = value; StatsUpdated?.Invoke(this); } get { return _buffCritChance; } }

    private float _buffCritDamage = 0;
    public float buffCritDamage { set { _buffCritDamage = value; StatsUpdated?.Invoke(this); } get { return _buffCritDamage; } }

    private float _buffBlockChance = 1;
    public float buffBlockChance { set { _buffBlockChance = value; StatsUpdated?.Invoke(this); } get { return _buffBlockChance; } }

    private float _buffHitChance = 1;
    public float buffHitChance { set { _buffHitChance = value; StatsUpdated?.Invoke(this); } get { return _buffHitChance; } }

    private float _buffArmorFactor = 0;
    public float buffArmorFactor { set { _buffArmorFactor = value; StatsUpdated?.Invoke(this); } get { return _buffArmorFactor; } }

    private float _buffAbilityRange = 1;
    public float buffAbilityRange { set { _buffAbilityRange = value; StatsUpdated?.Invoke(this); } get { return _buffAbilityRange; } }

    private float _buffHeal = 1;
    public float buffHeal { set { _buffHeal = value; StatsUpdated?.Invoke(this); } get { return _buffHeal; } }

    private float _buffMoveRange = 1;
    public float buffMoveRange { set { _buffMoveRange = value; StatsUpdated?.Invoke(this); } get { return _buffMoveRange; } }


    public void SetUp()
    {
        activeLifeTimeLimit = lifetimeLimit;
    }

    public virtual void Import(string[] tableRow)
    {
        SoulDrop = Int32.Parse(tableRow[3]);
        _autoManaGeneration = Int32.Parse(tableRow[4]);
        _lifetimeLimit = Int32.Parse(tableRow[5]);
        _baseDamage = float.Parse(tableRow[6], CultureInfo.InvariantCulture.NumberFormat);
        _baseCritChance = float.Parse(tableRow[7], CultureInfo.InvariantCulture.NumberFormat);
        _baseCritDamage = float.Parse(tableRow[8], CultureInfo.InvariantCulture.NumberFormat);
        _baseAbilityRange = float.Parse(tableRow[9], CultureInfo.InvariantCulture.NumberFormat);
        _armorFactor = float.Parse(tableRow[10], CultureInfo.InvariantCulture.NumberFormat);
        _blockChance = float.Parse(tableRow[11], CultureInfo.InvariantCulture.NumberFormat);
    }

}
