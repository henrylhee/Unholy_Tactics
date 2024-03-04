using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class XpBar : MonoBehaviour
{
    [SerializeField]
    private XpData xpData;

    [HideInInspector]
    public UnityEvent<int> LevelUp;

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField] private float fillSpeed = 0.05f;
    public int currentLevel { private set; get; } = 1;

    public void Fill()
    {
        List<int> xpToLevelUp = xpData.GetXpList(); 

        float currentXp = 0;

        int levelXp = 0;
        int playerXp = SaveState.Instance.data.playerXp;
        Debug.Log("XP in xpbar: " + playerXp);
        for (int i = 0; i < xpData.GetMaxLevel(); i++)
        {
            levelXp += xpToLevelUp[i];
            if (levelXp > playerXp)
            {
                currentXp = playerXp - (levelXp - xpToLevelUp[i]);
                currentLevel = i + 1;
                break;
            }
            else if(i+1 == xpData.GetMaxLevel() && playerXp >= levelXp)
            {
                currentXp = playerXp - (levelXp - xpToLevelUp[i]);
                currentLevel = i + 1;
            }
        }

        float currentValue = currentXp;
        float targetValue = 0; // currentValue + Combat.Instance.xpGained;

        SaveState.Instance.data.playerXp = levelXp;

        StartCoroutine(fill());
        IEnumerator fill()
        {
            Slider bar = GetComponent<Slider>();
            bar.maxValue = xpToLevelUp[currentLevel - 1];
            bar.minValue = 0;

            while (currentValue < targetValue)
            {
                yield return null;

                currentXp += fillSpeed;
                currentValue += fillSpeed;
                bar.value = currentXp;
                text.text = Mathf.FloorToInt(currentXp) + " / " + xpToLevelUp[currentLevel - 1];

                if (currentXp > xpToLevelUp[currentLevel - 1])
                {
                    currentXp = 0;
                    currentLevel++;
                    if(currentLevel == xpData.GetMaxLevel())
                    {
                        LevelUp?.Invoke(currentLevel);
                        break;
                    }
                    bar.maxValue = xpToLevelUp[currentLevel - 1];
                    bar.minValue = 0;
                    LevelUp?.Invoke(currentLevel);
                }
            }
        }
    }
}
