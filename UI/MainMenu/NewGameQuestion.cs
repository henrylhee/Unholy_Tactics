using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameQuestion : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Yes()
    {
        SaveState.Instance.ResetProgress();
        SaveState.Instance.Save();
        SceneLoader.Instance.LoadScene("LevelSelection");
    }

    public void No()
    {
        Hide();
    }
}
