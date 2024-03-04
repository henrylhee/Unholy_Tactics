using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Soul : MonoBehaviour
{
    public int value = 0;
    [HideInInspector]
    public UnityEvent collected;

    public void SetUp(int value)
    {
        this.value = value;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MainUnit>() != null)
        {
            collected?.Invoke();
            Destroy(this.gameObject);
        }
    }
}
