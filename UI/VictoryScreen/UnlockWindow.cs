using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnlockWindow : MonoBehaviour
{
    [SerializeField]
    private Transform unlockContentHolder;
    [SerializeField]
    private UnlockTooltip tooltipTemplate;
    [SerializeField]
    private ActionSound winSound;

    public void Show()
    {
        gameObject.SetActive(true);

        GetComponent<AudioSource>().clip = winSound.audioClip;
        GetComponent<AudioSource>().outputAudioMixerGroup = winSound.group;
        GetComponent<AudioSource>().volume = winSound.volume;
        GetComponent<AudioSource>().Play();

        foreach(UnlockData data in Unlocks.instance.GetNextUnlock())
        {
            UnlockTooltip tooltip = Instantiate(tooltipTemplate);
            tooltip.transform.SetParent(unlockContentHolder, false);
            tooltip.SetUp(data);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OpenMainMenu()
    {
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    public void OpenLevelSelection()
    {
        SceneLoader.Instance.LoadScene("Levelselection");
    }
}
