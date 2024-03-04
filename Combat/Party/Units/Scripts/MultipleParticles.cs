using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MultipleParticles : MonoBehaviour
{
    public void StartParticles()
    {
        foreach(ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
        {
            p.Play();
        }

        foreach(VisualEffect visualEffect in GetComponentsInChildren<VisualEffect>())
        {
            visualEffect.Play();
        }
    }

    public void StopParticles()
    {
        foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
        {
            p.Stop();
        }

        foreach(VisualEffect visualeffect in GetComponentsInChildren<VisualEffect>())
        {
            visualeffect.Stop();
        }
    }
}
