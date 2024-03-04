using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryVFXScreen : MonoBehaviour
{
    [SerializeField]
    private VictoryScreen victoryScreen;
    [SerializeField]
    private GameObject vfxObject;
    [SerializeField]
    private ActionSound winSound;

    public void VictoryScreen()
    {
        EndVFX();
        victoryScreen.Show();
    }

    public void StartVFX()
    {
        vfxObject.gameObject.SetActive(true);
        gameObject.SetActive(true);
        PlayWinSound();
    }

    public void EndVFX()
    {
        vfxObject.gameObject.SetActive(false);
        gameObject.gameObject.SetActive(false);
    }

    public void PlayWinSound()
    {
        GameObject audio = new GameObject();
        audio.AddComponent<AudioSource>();

        audio.GetComponent<AudioSource>().clip = winSound.audioClip;
        audio.GetComponent<AudioSource>().outputAudioMixerGroup = winSound.group;
        audio.GetComponent<AudioSource>().volume = winSound.volume;
        audio.GetComponent<AudioSource>().Play();
    }
}
