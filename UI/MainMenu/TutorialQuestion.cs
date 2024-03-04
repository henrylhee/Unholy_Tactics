using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialQuestion : MonoBehaviour
{
    public void yes()
    {
        SceneLoader.Instance.LoadScene("Tutorial_c");
    }

    public void no()
    {
        SaveState.Instance.Load();
        SaveState.Instance.data.tutorialFinished = true;
        SaveState.Instance.Save();
        SceneLoader.Instance.LoadScene("LevelSelection");
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
