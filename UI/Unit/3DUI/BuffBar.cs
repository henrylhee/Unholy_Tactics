using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuffBar : MonoBehaviour
{
    public Image templateIcon;

    private Dictionary<Buff, Image> activeIcons = new Dictionary<Buff, Image>();

    public void AddBuff(Buff buff)
    {
        Image Icon = Instantiate(templateIcon);
        Icon.sprite = buff.icon;
        Icon.transform.SetParent(transform, false);

        activeIcons.Add(buff, Icon);
    }

    public void RemoveBuff(Buff buff) 
    {
        if (activeIcons.Keys.Contains(buff))
        {
            Image icon = activeIcons[buff];
            Destroy(icon.gameObject, 0.1f);

            activeIcons.Remove(buff);
        }
    }
}
