using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LockPosition : MonoBehaviour
{
    private void Update()
    {
        transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
    }
}
