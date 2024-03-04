using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStats : MonoBehaviour
{
    [SerializeField]
    private HealthBar healthbar;
    [SerializeField]
    private TextMeshProUGUI deathCount;

    public void UpdateHealthCount(int amount, int max)
    {
        healthbar.UpdateHealth(amount, max);
    }

    public void UpdateAutoDeathCount(int amount, int max)
    {
        if(amount == 0)
        {
            deathCount.text = "";
            return;
        }

        deathCount.text = amount.ToString() + "/" + max.ToString();
    }
}
