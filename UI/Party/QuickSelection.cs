using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuickSelection : MonoBehaviour
{
    private TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateIndex(int index)
    {
        text.text = index.ToString();
    }
}
