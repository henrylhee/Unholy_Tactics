using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LevelMap : MonoBehaviour
{
    [SerializeField]
    private List<UILevel> demoLevels;
    [SerializeField]
    private GameObject demoIcon;

    [SerializeField]
    private ModeQuestion question;
    [SerializeField]
    private VictoryMapScreen victory;

    private List<UILevel> levels;
    [SerializeField]
    private int defeatedLevels = -1;

    private void Start()
    {
        if(defeatedLevels <= -1)
        {
            defeatedLevels = SaveState.Instance.data.unlockedLevels;
        }
        
        levels = new List<UILevel>();
        levels.AddRange(GetComponentsInChildren<UILevel>());


        if(defeatedLevels == 0)
        {
            question.modeSelected.AddListener(OnModeDescision);
            question.Show();
        }
        else
        {
            LoadLevels();
        }
    }

    public void OnLevelSelected(UILevel level)
    {
        if (Unlocks.instance == null)
        {
            GameObject unlocks = new GameObject("Unlocks");
            unlocks.AddComponent<Unlocks>();
        }

        List<UILevel> unlocked = new List<UILevel>();
        for(int i = 0;i < levels.IndexOf(level); i++)
        {
            unlocked.Add(levels[i]);
        }

        Unlocks.instance.SetUp(unlocked,level,levels.IndexOf(level));
        SceneLoader.Instance.LoadScene(level.LevelName);
    }

    public void UnlockLevels()
    {
        if (defeatedLevels >= levels.Count)
        {
            victory.Show();
            return;
        }

        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].SetUp(i < defeatedLevels);
            levels[i].Clicked.AddListener(OnLevelSelected);
        }

        levels[defeatedLevels].Unlock();
    }

    public void UnlockDemoLevels()
    {
        if (defeatedLevels >= demoLevels.Count)
        {
            victory.Show();
            return;
        }

        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].SetUp(i < levels.IndexOf(demoLevels[defeatedLevels]));
            levels[i].Clicked.AddListener(OnLevelSelected);
        }

        levels[levels.IndexOf(demoLevels[defeatedLevels])].Unlock();
    }

    public void OnReturnToMain()
    {
        SceneLoader.Instance.LoadScene("Scenes/NewUI/MainMenu");
    }

    private void LoadLevels()
    {
        if (!SaveState.Instance.data.demoMode)
        {
            UnlockLevels();
        }
        else
        {
            UnlockDemoLevels();
        }

        demoIcon.gameObject.SetActive(SaveState.Instance.data.demoMode);
    }

    private void OnModeDescision(bool demo)
    {
        SaveState.Instance.Load();
        SaveState.Instance.data.demoMode = demo;
        SaveState.Instance.Save();

        LoadLevels();
    }
}
