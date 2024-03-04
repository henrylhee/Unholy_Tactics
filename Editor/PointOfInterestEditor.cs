using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PointOfInterest))]
[CanEditMultipleObjects]
public class PointOfInterestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PointOfInterest t = (PointOfInterest)target;
        if(GUILayout.Button("Print Cover count"))
        {
            List<Unit> enemies = new List<Unit>();

            foreach(Unit unit in FindObjectsOfType<PlayerUnit>())
            {
                enemies.Add(unit);
            }
        }
    }
}
