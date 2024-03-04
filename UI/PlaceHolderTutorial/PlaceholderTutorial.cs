using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderTutorial : MonoBehaviour
{
    [SerializeField]
    private TutorialInformations infos;

    public void Begin()
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
        infos.StartTutorial();
    }

    public void OnEnd()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
