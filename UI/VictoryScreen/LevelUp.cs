using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public void SetUp()
    {
        LevelUpUnit unit = GetComponentInChildren<LevelUpUnit>();

        unit.SetUp();
    }
}
