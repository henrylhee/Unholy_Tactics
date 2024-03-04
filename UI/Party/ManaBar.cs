using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField]
    private Image manaBar;
    [SerializeField]
    private TextMeshProUGUI manaText;

    public void UpdateValue(float value, float max)
    {
        manaBar.fillAmount = value / max;
        manaText.text = value.ToString() + "/" + max.ToString();
    }
}
