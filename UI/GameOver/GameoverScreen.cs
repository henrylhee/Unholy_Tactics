using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverScreen : MonoBehaviour
{
    [SerializeField]
    private ActionSound looseSound;

    public void Retry()
    {
        SceneLoader.QueueTransition();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Show()
    {
        GetComponent<AudioSource>().clip = looseSound.audioClip;
        GetComponent<AudioSource>().outputAudioMixerGroup = looseSound.group;
        GetComponent<AudioSource>().volume = looseSound.volume;
        GetComponent<AudioSource>().Play();

        gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void Hide()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
