using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialLevel : MonoBehaviour
{
    [SerializeField]
    private TutorialQuestion tut;

    public void Click()
    {
        tut.Show();
    }
}
