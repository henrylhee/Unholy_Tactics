using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private TutorialQuestion tut;
    [SerializeField]
    private NewGameQuestion newGame;
    [SerializeField]
    private OptionsMenu options;

    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private AudioSource buttonAudio;

    public void Awake()
    {
        buttonAudio = GetComponent<AudioSource>();
    }

    public void Start()
    {
        Time.timeScale = 1;
        
        masterMixerGroup.audioMixer.SetFloat("MasterVolume", Mathf.Log10(
            SaveState.Instance.data.isMasterMuted ? 0.001f : SaveState.Instance.data.masterVolume) * 20);
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(
            SaveState.Instance.data.isMusicMuted ? 0.001f :SaveState.Instance.data.musicVolume) * 20);
        sfxMixerGroup.audioMixer.SetFloat("SfxVolume", Mathf.Log10(
            SaveState.Instance.data.isSfxMuted ? 0.001f :SaveState.Instance.data.soundVolume) * 20);
        
        Screen.SetResolution(SaveState.Instance.data.xResolution, SaveState.Instance.data.yResolution, SaveState.Instance.data.isFullscreen);
        Screen.fullScreen = SaveState.Instance.data.isFullscreen;
    }

    public void Continue()
    {
        buttonAudio.Play();
        SceneLoader.Instance.LoadScene("LevelSelection");
        //OnTutorialFinished();
    }

    public void NewGame()
    {
        buttonAudio.Play();
        SaveState.Instance.Load();
        if(!SaveState.Instance.data.tutorialFinished)
        {
            tut.Show();
        }
        else
        {
            newGame.Show();
        }

        
    }

    public void OpenSettings()
    {
        buttonAudio.Play();
        options.Show();
    }

    public void OpenCredits()
    {
        buttonAudio.Play();
        SceneLoader.Instance.LoadScene("Scenes/NewUI/Credits");
    }
    
    public void Quit()
    {
        buttonAudio.Play();
        Application.Quit();
    }

    public void OnTutorialFinished()
    {
        buttonAudio.Play();
        SceneLoader.Instance.LoadScene("LevelSelection");
    }

    public void OnDeleteSave()
    {
        buttonAudio.Play();
        SaveState.Instance.ResetProgress();
    }

    public void OpenTutorial()
    {
        SceneLoader.Instance.LoadScene("Tutorial_c");
    }
}
