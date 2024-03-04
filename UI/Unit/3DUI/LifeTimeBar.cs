using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTimeBar : MonoBehaviour
{
    [SerializeField]
    private LifeTimeSkull skullTemplate;

    private List<LifeTimeSkull> skulls = new List<LifeTimeSkull>();

    public void SetSkulls(int value)
    {
        while (value > skulls.Count)
        {
            LifeTimeSkull skull = Instantiate(skullTemplate, transform, false);
            skull.SetUp();
            skulls.Add(skull);
        }

        while (value < skulls.Count)
        {
            skulls[skulls.Count - 1].Hide();
            skulls.Remove(skulls[skulls.Count - 1]);
        }

        if (skulls.Count == 1)
        {
            skulls[0].Blink();
        }
    }
}
