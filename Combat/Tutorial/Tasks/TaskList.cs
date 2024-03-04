using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskList : MonoBehaviour
{
    private List<TaskPoint> points = new List<TaskPoint>();

    private void Awake()
    {
        foreach(var point in GetComponentsInChildren<TaskPoint>())
        {
            points.Add(point);
        }
    }

    public void CrossPoint(int point, int subPoint)
    {
        point--;
        subPoint--;

        if (point < points.Count) 
        {
            points[point].CrossPoint(subPoint);
        }
    }
}
