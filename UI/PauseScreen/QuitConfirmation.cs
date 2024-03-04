using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitConfirmation : MonoBehaviour
{
    public void OnYes()
    {
        Time.timeScale = 1;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();

    }

    public void OnNo()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        GetComponent<Animator>().Play("QuitConfirmationIntro");
    }

    public void Hide()
    {
        GetComponent<Animator>().Play("QuitConfirmationOutro");
    }
}
