using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionStat : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI stat;

    public void SetUp(float value, float valueTwo = 0, string extraSign = "")
    {
        stat.text = value.ToString();

        if(valueTwo > 0 && valueTwo != value)
        {
            stat.text += " - " + valueTwo.ToString();
        }

        stat.text += extraSign;
    }
}
