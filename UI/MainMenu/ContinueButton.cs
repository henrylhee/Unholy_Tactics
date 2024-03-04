using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    private void Start()
    {
        if (!SaveState.Instance.data.tutorialFinished)
        {
            gameObject.SetActive(false);
        }
    }
}
