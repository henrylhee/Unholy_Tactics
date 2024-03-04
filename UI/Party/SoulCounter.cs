using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoulCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI soulText; 

    public void UpdateCounter(int value)
    {
        soulText.text = value.ToString();
    }
}
