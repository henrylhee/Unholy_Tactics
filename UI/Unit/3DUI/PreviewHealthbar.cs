using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewHealthbar : MonoBehaviour
{
    [SerializeField]
    private HealthBar maxDamageBar;
    [SerializeField]
    private HealthBar minDamageBar;
    [SerializeField]
    private HealthBar postMinDamageBar;

    public void UpdateValue(float maxhealth, float currentHealth, float minDamage, float maxDamage)
    {
        return;
        if(maxDamage <= 0)
        {
            gameObject.SetActive(false);
        }
        gameObject.SetActive(true);

        postMinDamageBar.UpdateHealth(currentHealth - maxDamage, maxhealth);
        minDamageBar.UpdateHealth(currentHealth - minDamage, maxhealth);
        maxDamageBar.UpdateHealth(currentHealth, maxhealth);
    }
}
