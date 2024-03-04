using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class Buffs
{
    private List<Buff> activeBuffs = new List<Buff>();
    
    public Buff AddBuff(Buff buff, Unit source, bool createCopy = true)
    {
        if (createCopy)
        {
            Buff instance = GameObject.Instantiate(buff);
            instance.Copy(buff);
            buff = instance;
        }

        /* if (activeBuffs.Contains(buff))
        {
            return;
        }*/

        buff.OnApply(source);
        activeBuffs.Add(buff);

        return buff;
    }

    public void RemoveBuff(Buff buff, Unit source)
    {
        if (!activeBuffs.Contains(buff))
        {
            return;
        }

        buff.OnRemove(source);
        activeBuffs.Remove(buff);
    }

    public void ClearBuffs(Unit source)
    {
        while(activeBuffs.Count > 0)
        {
            RemoveBuff(activeBuffs[0], source);
        }
    }
    
    public void OnTakeDamage(Unit source, int damage)
    {
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            activeBuffs[i].OnTakeDamage(source, damage);
        }
    }

    public int GetDamageModifier(Unit source)
    {
        int total = 0;
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            total += activeBuffs[i].TakeDamageModifier(source);
        }
        return total;
    }


    public void OnHealDamage(Unit source)
    {
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            activeBuffs[i].OnHealDamage(source);
        }
    }

    public int GetHealModifier(Unit source)
    {
        int total = 0;
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            total += activeBuffs[i].HealDamageModifier(source);
        }
        return total;
    }

    public void OnTurnStart(Unit source)
    {
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            activeBuffs[i].OnTurnStart(source);
        }
    }

    public void OnTurnEnd(Unit source)
    {
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            activeBuffs[i].OnTurnEnd(source);
        }
    }
}
