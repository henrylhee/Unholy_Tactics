using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Buff : ScriptableObject
{
    [Header("Display Configs")]
    [SerializeField] [TextArea] protected string description;
    public string Description => description;

    public Sprite icon;
    
    [Header("Buff")]
    [SerializeField]
    protected int duration;
    [SerializeField]
    private ParticleSystem buffParticles;

    private int activeDuration;
    public int ActiveDuration => activeDuration;

    private ParticleSystem activeBuffParticles;

    public void Copy(Buff buff)
    {
        this.description = buff.description;
        this.icon = buff.icon;
        this.duration = buff.duration;
    }

    public virtual void OnApply(Unit caster)
    {
        activeDuration = duration;

        activeBuffParticles = Instantiate(buffParticles);
        activeBuffParticles.transform.SetParent(caster.transform);
        activeBuffParticles.transform.position = caster.transform.position + new Vector3(0,1,0);
        activeBuffParticles.Play();
    }

    public virtual void OnRemove(Unit caster)
    {
        if(activeBuffParticles != null)
        {
            Destroy(activeBuffParticles.gameObject,3);
            activeBuffParticles.Stop();
        }
    }


    public virtual void OnTakeDamage(Unit caster, int damage)
    {
        
    }

    public virtual int TakeDamageModifier(Unit caster)
    {
        return 0;
    }


    public virtual void OnHealDamage(Unit caster)
    {
        
    }

    public virtual int HealDamageModifier(Unit caster)
    {
        return 0;
    }

    public virtual void OnTurnStart(Unit caster)
    {
        activeDuration--;
        if (activeDuration <= 0)
        {
            caster.RemoveBuff(this);
        }
    }

    public virtual void OnTurnEnd(Unit caster)
    {
        
    }
}
