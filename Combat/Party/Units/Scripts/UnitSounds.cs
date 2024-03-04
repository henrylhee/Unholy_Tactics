using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSounds : MonoBehaviour
{
    [SerializeField]
    private AudioSource hurt;
    [SerializeField]
    private AudioSource die;


    public void PlayDamageSound()
    {
        CreateAudioInstance(hurt).Play();
    }

    public void PlayDeathSound()
    {
        CreateAudioInstance(die).Play();
    }

    private AudioSource CreateAudioInstance(AudioSource toInstantiate)
    {
        AudioSource instance = Instantiate(toInstantiate);
        instance.transform.position = toInstantiate.transform.position;
        Destroy(instance.gameObject, 5);
        return instance;
    }
}
