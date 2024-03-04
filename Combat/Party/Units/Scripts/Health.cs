using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Health
{
    [SerializeField]
    private int maxHp;
    private UnitSounds sounds;

    public int CurrentHp => currentHp;
    private int currentHp;
    public int MaxHp { set { maxHp = value; } get { return maxHp; } }

    [HideInInspector]
    public UnityEvent Die = new UnityEvent();
    [HideInInspector]
    public UnityEvent<int,int> HealthChanged = new UnityEvent<int, int>();
    [HideInInspector]
    public UnityEvent<int> DamageTaken = new UnityEvent<int>();
    

    public void SetUp(UnitSounds sounds)
    {
        this.sounds = sounds;
        currentHp = maxHp;
    }

    public bool CanSurviveDamage(int amount)
    {
        return currentHp > amount;
    }

    public void TakeDamage(int amount)
    {
        amount = Mathf.Clamp(amount, 0, int.MaxValue);
        if(amount <= 0)
        {
            return;
        }

        currentHp = Mathf.Clamp(currentHp - amount, 0, maxHp);
        DamageTaken?.Invoke(amount);
        HealthChanged.Invoke(currentHp, maxHp);
        if (currentHp <= 0)
        {
            sounds?.PlayDeathSound();
            Die?.Invoke();
            return;
        }

        sounds?.PlayDamageSound(); 
    }

    public void Heal(int amount)
    {
        currentHp = Mathf.Clamp(currentHp + amount, currentHp, maxHp);
        HealthChanged?.Invoke(currentHp, maxHp);
    }

    public int GetCurrentHealth()
    {
        return currentHp;
    }

    public int GetMaxHealth()
    {
        return maxHp;
    }
}
