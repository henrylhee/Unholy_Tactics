using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskPoint : MonoBehaviour
{
    private TextMeshProUGUI[] texts;

    private void Awake()
    {
        texts = GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void CrossPoint(int subPoint)
    {
        subPoint++;

        string previously = texts[subPoint].text;
        texts[subPoint].text = "<s>" + previously + "</s>";

        bool done = true;
        foreach (TextMeshProUGUI text in texts)
        {
            if (!text.text.Contains("<s>") && text != texts[0])
            {
                done = false;
            }
            previously = texts[subPoint].text;
        }

        if (done)
        {
            foreach (TextMeshProUGUI text in texts)
            {
                texts[subPoint].text = "<s>" + previously + "</s>";
                return;
            }
        }
    }
}

