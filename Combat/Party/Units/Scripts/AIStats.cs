using System;
using UnityEngine;

public class AIStats : UnitStats
{
    [SerializeField] private Soul soul;

    public override void Import(string[] tableRow)
    {
        base.Import(tableRow);
        soul.value = Int32.Parse(tableRow[11]);
    }
}
