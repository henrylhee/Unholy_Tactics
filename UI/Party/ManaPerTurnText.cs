using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManaPerTurnText : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float value = 0;

        foreach(Unit u in FindObjectsOfType<Unit>())
        {
            if(u.health.CurrentHp > 0)
            {
                value += u.stats.autoManaGeneration;
            }
        }

        text.text = "Mana per turn:" + value.ToString();
    }
}
