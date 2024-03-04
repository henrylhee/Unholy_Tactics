using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionCost : MonoBehaviour
{
    public void SetValue(int value)
    {
        gameObject.SetActive(true);
        GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();

        if(value <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
