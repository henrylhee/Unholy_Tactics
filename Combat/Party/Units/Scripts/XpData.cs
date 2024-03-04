using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "XpData", menuName = "XpData", order = 1)]
public class XpData : ScriptableObject
{
    [SerializeField]
    private float startValue;
    [SerializeField]
    private float quadraticGrowth;
    [SerializeField]
    private float linearGrowth;
    [SerializeField]
    private int maxLevel;

    public List<int> GetXpList()
    {
        List<int> result = new List<int>();
        for (int i = 1; i < maxLevel; i++)
        {
            result.Add(Mathf.RoundToInt(i * i * quadraticGrowth + i * linearGrowth + startValue));
        }
        return result;
    }

    public int GetMaxLevel()
    {
        return maxLevel;
    }
}
