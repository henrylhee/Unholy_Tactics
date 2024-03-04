using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevel : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<UILevel> Clicked;
    
    [SerializeField]
    private GameObject evilArea;
    [SerializeField]
    private Button button;
    [SerializeField]
    private Image goodArea;
    [SerializeField]
    private GameObject levelLabel;

    [SerializeField]
    private string LevelLabel;

    [SerializeField]
    public string LevelName;

    public List<UnlockData> unlocks;

    private void Awake()
    {
        levelLabel.SetActive(false);
    }

    public void SetUp(bool beaten)
    {
        levelLabel.gameObject.SetActive(false);
        levelLabel.transform.SetParent(transform.parent);
        levelLabel.GetComponentInChildren<TextMeshProUGUI>().text = LevelLabel;
        if (beaten)
        {
            Beat();
        }
        else
        {
            Lock();
        }
    }

    public void Lock()
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        evilArea.SetActive(false);
        button.interactable = false;
    }

    public void Unlock()
    {
        levelLabel.SetActive(true);
        goodArea.gameObject.SetActive(true);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        button.interactable = true;
    }

    public void Beat()
    {
        Lock();
        evilArea.SetActive(true);
    }

    public void LoadLevel()
    {
        Clicked?.Invoke(this);
    }
}

public enum UnitType { Main, Melee, Range, Mage, Support}

[System.Serializable]
public class UnlockData
{
    public Action action;
    public UnitType unit;
}
