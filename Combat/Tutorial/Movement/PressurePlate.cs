using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public UnityEvent OnTrigger;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MainUnit>() != null && !triggered)
        {
            triggered = true;
            OnTrigger?.Invoke();
        }
    }
}
