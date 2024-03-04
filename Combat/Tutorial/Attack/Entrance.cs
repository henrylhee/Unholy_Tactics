using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entrance : MonoBehaviour
{
    public UnityEvent OnEnter;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MainUnit>() != null && !triggered)
        {
            triggered = true;
            OnEnter?.Invoke();
        }
    }
}
