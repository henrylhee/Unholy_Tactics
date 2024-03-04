using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocks : MonoBehaviour
{
    public static Unlocks instance;

    private List<UnlockData> unlocked;
    private List<UnlockData> aboutToUnlock;
    public int currentLevel = 0;

    private void Awake()
    {
        if(Unlocks.instance != null)
        {
            return;
        }

        Unlocks.instance = this;
        DontDestroyOnLoad(this);
    }

    public void SetUp(List<UILevel> unlockedLevel, UILevel currentLevel, int unlockedLevels)
    {
        this.currentLevel = unlockedLevels;
        unlocked = new List<UnlockData>();
        foreach(UILevel level in unlockedLevel)
        {
            unlocked.AddRange(level.unlocks);
        }

        aboutToUnlock = new List<UnlockData>();
        aboutToUnlock.AddRange(currentLevel.unlocks);
    }

    public List<Action> GetUnlockedActions(UnitType type)
    {
        List<Action> result = new List<Action>();
        foreach(UnlockData d in unlocked)
        {
            if(d.unit == type)
            {
                result.Add(d.action);
            }
        }

        return result;
    }

    public List<UnlockData> GetNextUnlock()
    {
        return aboutToUnlock;
    }
}
