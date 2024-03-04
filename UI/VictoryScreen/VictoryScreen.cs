using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField]
    private UnlockWindow unlockWindow;
    [SerializeField]
    private ActionSound popSound;

    public void LoadNextScene()
    {
        Time.timeScale = 1;
        SceneLoader.Instance.LoadScene("LevelSelection");
    }
    
    public void MainMenu()
    {
        Hide();
        Time.timeScale = 1;
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SaveState.Instance.Load();
        SaveState.Instance.data.unlockedLevels = Unlocks.instance.currentLevel + 1;
        SaveState.Instance.Save();

        if(Unlocks.instance != null)
        {
            unlockWindow.Show();
        }
    }

    public void Hide()
    {
        Time.timeScale = 1;
    }

    public void OnPop()
    {
        //GameObject audio = new GameObject();
        //audio.AddComponent<AudioSource>();

        //audio.GetComponent<AudioSource>().clip = popSound.audioClip;
        //audio.GetComponent<AudioSource>().outputAudioMixerGroup = popSound.group;
        //audio.GetComponent<AudioSource>().volume = popSound.volume;
        //audio.GetComponent<AudioSource>().Play();
    }

    public void OnAnimationFinished()
    {

    }
}