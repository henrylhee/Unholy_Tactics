using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConcedeConfirmation : MonoBehaviour
{
    public void OnYes()
    {
        Time.timeScale = 1;
        SceneLoader.Instance.LoadScene("LevelSelection");
    }

    public void OnNo()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        GetComponent<Animator>().Play("ConcedeConfirmationIntro");
    }

    public void Hide()
    {
        GetComponent<Animator>().Play("ConcedeConfirmationOutro");
    }
}
