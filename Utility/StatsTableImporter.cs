using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsTableImporter
{
    void SetUp(string ID, TextAsset table, Unit unit)
    {
        Import(ID, table, unit);
    }

    public void Import(string ID, TextAsset table, Unit importUnit)
    {
        UnitStats statsToUpdate = null;
        Health healthToUpdate = null;

        statsToUpdate = importUnit.stats;
        healthToUpdate = importUnit.health;

        var statRow = Array.Find(table.text.Split('\n'), row => row.Split(',')[0] == ID);

        if (statRow == null)
        {
            Debug.LogError("No row with ID '" + ID + "' was found in table '" + table.name + "'. Import failed!");
            return;
        }

        var stats = statRow.Split(',');
        healthToUpdate.MaxHp = Int32.Parse(stats[2]);
        healthToUpdate.Heal(healthToUpdate.MaxHp);
        statsToUpdate.Import(stats);
    }
}
