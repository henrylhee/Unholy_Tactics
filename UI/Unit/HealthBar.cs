using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private bool showMax = false;

    private float targetValue;

    private void Awake()
    {
        if(healthBar == null)
        {
            return;
        }

        healthBar.fillAmount = 1;
    }

    public void UpdateHealth(float value, float maxhealth)
    {
        if(maxhealth == 0)
        {
            return;
        }

        targetValue = value / maxhealth;

        if (text != null)
        {
            text.text = value.ToString();
            if (showMax)
            {
                text.text += "/" + maxhealth.ToString();
            }
        }
    }

    private void Update()
    {
        if(healthBar == null)
        {
            return;
        }

        if(healthBar.fillAmount < targetValue)
        {
            healthBar.fillAmount = Mathf.Clamp(healthBar.fillAmount + 0.01f, 0, targetValue);
        }
        else if (healthBar.fillAmount > targetValue)
        {
            healthBar.fillAmount = Mathf.Clamp(healthBar.fillAmount - 0.01f, targetValue, 1);
        }
    }
}
