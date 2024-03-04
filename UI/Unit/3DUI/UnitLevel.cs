using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitLevel : MonoBehaviour
{
    [SerializeField]
    private List<LevelInfo> levelInfos;

    public void SetLevel(int level)
    {
        GetComponent<Image>().color = levelInfos[level - 1].levelColor;
        GetComponent<Image>().sprite = levelInfos[level - 1].sprite;

        //GetComponentInChildren<TextMeshProUGUI>().text = level.ToString();
    }
}

[System.Serializable]
public class LevelInfo
{
    public Sprite sprite;
    public Color levelColor;
}

//
