using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Action
{
    [Header("Attack")]

    [SerializeField]
    protected int healing;

    [SerializeField]
    protected int minDamage;
    [SerializeField]
    protected int maxDamage;
    [SerializeField]
    protected float critDamage;
    [SerializeField]
    protected float critChance;
    [SerializeField]
    protected float hitChance;
    [Header(" ")]

    [SerializeField]
    protected float range;

    public override void SetUp(Unit caster)
    {
        base.SetUp(caster);
        _range = range;
    }

    public DamageInformation CalculateRealDmg(Unit caster, Unit target)
    {
        float damage = Random.Range(GetMinDamage(caster), GetMaxDamage(caster));
        damage -= Mathf.Clamp( target.stats.armorFactor + target.stats.buffArmorFactor, 0, 42);

        DamageInformation damageInfo = new DamageInformation();
        if (Random.Range(0f, 1f) <= GetCritChance(caster))
        {
            damageInfo.damage = Mathf.RoundToInt(GetCritDamage(damage, caster));
            damageInfo.crit = true;
        }
        else
        {
            damageInfo.damage = Mathf.RoundToInt(damage);
        }

        return damageInfo;
    }


    public float GetAverageDamage(Unit caster, Unit target)
    {
        float total = 0;
        for(int i = 0;i < 20; i++)
        {
            total += CalculateRealDmg(caster, target).damage;
        }

        return total / 20;
    }

    protected override void UpdatePreview(PlayerUnit caster)
    {
        List<Unit> targets = GetTargetUnits(caster);

        if(targets != null)
        {
            foreach (Unit target in targets)
            {
                float minDamage = GetMinDamage(caster);
                float maxDamage = GetMaxDamage(caster);

                minDamage -= Mathf.Clamp( target.stats.armorFactor + target.stats.buffArmorFactor, 0f, 42f);
                maxDamage -= Mathf.Clamp( target.stats.armorFactor + target.stats.buffArmorFactor, 0, 42);

                target.PreviewTakeDamage(Mathf.RoundToInt(minDamage), Mathf.RoundToInt(maxDamage));
            }
        }
      
        base.UpdatePreview(caster);
    }

    public override void ShutDownPreview(PlayerUnit caster)
    {
        caster.PreviewTakeDamage(0, 0);
        base.ShutDownPreview(caster);
    }

    protected bool IsAttackSuccessful(Unit caster, Unit target)
    {
        if(Random.Range(0f, 1f) <= GetHitChance(caster))
        {
            if(Random.Range(0f, 1f) > target.stats.blockChance)
            {
                return true;
            }
            else
            {
                Debug.Log("Attack was blocked!");
                return false;
            }
        }
        target.unitVFX.AttackMissed();
        Debug.Log("Hit did miss!");
        return false;
    }



    public override float GetMinDamage(Unit caster)
    {
        return Mathf.RoundToInt(minDamage + caster.stats.baseDamage + Mathf.Max(caster.stats.buffDamage, 0));
    }

    public override float GetMaxDamage(Unit caster)
    {
        return maxDamage + caster.stats.baseDamage + Mathf.Max(caster.stats.buffDamage, 0);
    }

    public override float GetCritChance(Unit caster)
    {
        return critChance * caster.stats.baseCritChance * caster.stats.buffCritChance;
    }

    public override float GetCritDamage(float baseDamage, Unit caster)
    {
        return GetMaxDamage(caster) + critDamage;
    }

    public override float GetHitChance(Unit caster)
    {
        return hitChance * caster.stats.hitChance * Mathf.Max(caster.stats.buffHitChance, 0);
    }

    public override float GetHealing(Unit caster)
    {
        return healing * Mathf.Max(caster.stats.buffHeal, 0);
    }
}

public class DamageInformation
{
    public int damage;
    public bool crit;
}