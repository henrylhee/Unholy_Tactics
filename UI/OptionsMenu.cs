using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Toggle masterToggle;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle sfxToggle;

    [SerializeField] private TMP_Dropdown resolutionOptions;
    
    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    

    public void Show()
    {
        gameObject.SetActive(true);
        GetComponent<Animator>().Play("OptionsIntro");
    }

    public void Hide()
    {
        GetComponent<Animator>().Play("OptionsOutro");
    }

    // Start is called before the first frame update
    void Start()
    {
        fullscreenToggle.isOn = SaveState.Instance.data.isFullscreen;
        masterSlider.value = SaveState.Instance.data.masterVolume;
        masterToggle.isOn = !SaveState.Instance.data.isMasterMuted;
        musicSlider.value = SaveState.Instance.data.musicVolume;
        musicToggle.isOn = !SaveState.Instance.data.isMusicMuted;
        sfxSlider.value = SaveState.Instance.data.soundVolume;
        sfxToggle.isOn = !SaveState.Instance.data.isSfxMuted;
        ConfigureResolutionOptions();
    }

    private void ConfigureResolutionOptions()
    {
        foreach (var res in Screen.resolutions)
        {
            resolutionOptions.options.Add( new TMP_Dropdown.OptionData("" + res.width + "x" + res.height));
        }

        int resolutionIndex = resolutionOptions.options.FindIndex(option => option.text ==
                                                                            "" + SaveState.Instance.data.xResolution +
                                                                            "x" + SaveState.Instance.data.yResolution);
        resolutionOptions.value = resolutionIndex >= 0 ? resolutionIndex : resolutionOptions.options.Count - 1;
    }
    
    public void ToggleFullscreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
        SaveState.Instance.data.isFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnMasterValueChanged(float value)
    {
        if (!SaveState.Instance.data.isMasterMuted)
            masterMixerGroup.audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        SaveState.Instance.data.masterVolume = value;
    }
    
    public void OnMusicValueChanged(float value)
    {
        if (!SaveState.Instance.data.isMusicMuted)
            musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        SaveState.Instance.data.musicVolume = value;
    }
    
    public void OnSfxValueChanged(float value)
    {
        if (!SaveState.Instance.data.isSfxMuted)
            sfxMixerGroup.audioMixer.SetFloat("SfxVolume", Mathf.Log10(value) * 20);
        SaveState.Instance.data.soundVolume = value;
    }

    public void OnResolutionChanged(int idx)
    {
        int xResolution = Int32.Parse(resolutionOptions.options[idx].text.Split("x")[0]);
        int yResolution = Int32.Parse(resolutionOptions.options[idx].text.Split("x")[1]);
        
        Screen.SetResolution(xResolution, yResolution, SaveState.Instance.data.isFullscreen);
        SaveState.Instance.data.xResolution = xResolution;
        SaveState.Instance.data.yResolution = yResolution;
    }

    public void OnMasterToggle(bool isOn)
    {
        masterMixerGroup.audioMixer.SetFloat("MasterVolume", 
            Mathf.Log10(isOn ? SaveState.Instance.data.masterVolume : 0.001f) * 20);
        SaveState.Instance.data.isMasterMuted = !isOn;
    }
    
    public void OnMusicToggle(bool isOn)
    {
        masterMixerGroup.audioMixer.SetFloat("MusicVolume", 
            Mathf.Log10(isOn ? SaveState.Instance.data.musicVolume : 0.001f) * 20);
        SaveState.Instance.data.isMusicMuted = !isOn;
    }
    
    public void OnSfxToggle(bool isOn)
    {
        masterMixerGroup.audioMixer.SetFloat("SfxVolume", 
            Mathf.Log10(isOn ? SaveState.Instance.data.soundVolume : 0.001f) * 20);
        SaveState.Instance.data.isSfxMuted = !isOn;
    }
    
    public void OpenMainMenu()
    {
        SceneLoader.Instance.LoadScene("Scenes/NewUI/MainMenu");
    }
}
