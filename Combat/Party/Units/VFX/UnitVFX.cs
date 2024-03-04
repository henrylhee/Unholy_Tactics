using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitVFX : MonoBehaviour
{
    [SerializeField]
    private GameObject miss;
    [SerializeField]
    private MultipleParticles magic;
    [SerializeField]
    private DamageVFX damage;

    [SerializeField]
    private MultipleParticles floorVFX;
    [SerializeField]
    private ParticleSystem friendlyBlood;
    [SerializeField]
    private ParticleSystem enemyBlood;
    [SerializeField]
    private MultipleParticles FallVFX;
    [SerializeField]
    private ParticleSystem stepVFX;
    [SerializeField]
    private ParticleSystem poisonCloud;

    private void Awake()
    {
        foreach(MultipleParticles m in GetComponentsInChildren<MultipleParticles>())
        {
            m.StopParticles();
        }

        floorVFX?.StartParticles();
    }

    public void PlayActionParticles(CastVFX vfx)
    {
        switch(vfx)
        {
            case CastVFX.MAGIC:
                Magic();
                break;
            case CastVFX.PoisonCloud:
                PlayPoisonCloud();
                break;
        }
    }

    public void StartFloorVFX()
    {
        floorVFX?.StartParticles();
    }

    public void EndFloorVFX()
    {
        floorVFX?.StopParticles();
    }


    public void AttackMissed()
    {
        miss?.GetComponent<ParticleSystem>().Play();
    }

    public void Magic()
    {
        magic?.StartParticles();
    }

    public void DamageTaken(int damage, bool crit)
    {
        if (crit)
        {
            this.damage?.PlayAnimation(damage, DamageVFXType.Crit);
        }
        else
        {
            this.damage?.PlayAnimation(damage, DamageVFXType.Damage);
        }
    }

    public void Healed(int healing)
    {
        this.damage?.PlayAnimation(healing, DamageVFXType.Healing);
    }

    public void PlayFriendlyBlood()
    {
        friendlyBlood?.Play();
    }

    public void PlayEnemyBlood()
    {
        enemyBlood?.Play();
    }

    public void PlayFallVFX()
    {
        FallVFX?.StartParticles();
    }

    public void PlayStepVfx()
    {
        stepVFX?.Play();
    }

    public void PlayPoisonCloud()
    {
        poisonCloud?.Play();
    }
}
