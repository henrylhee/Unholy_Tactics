using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuffIcon : MonoBehaviour
{
    private Buff buff;
    public void SetUp(Buff buff)
    {
        this.buff = buff;

        GetComponent<UIInteractable>().tooltipText = buff.Description;
        GetComponentInChildren<Image>().sprite = buff.icon;
    }

    public void ShutDown()
    {
        Destroy(this.gameObject);
    }

    public bool IsFor(Buff buff)
    {
        return this.buff == buff;
    }
}
