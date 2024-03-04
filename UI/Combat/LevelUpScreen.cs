using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUpScreen : MonoBehaviour
{
    [SerializeField]
    private XpBar xpBar;
    [SerializeField]
    private TextMeshProUGUI levelText;



    public void Show()
    {
        xpBar.LevelUp.AddListener(OnLevelUp);
        xpBar.Fill();
        levelText.text = "Level " + xpBar.currentLevel;
    }

    private void OnLevelUp(int level)
    {
        levelText.text = "Level " + xpBar.currentLevel;
    }
}
