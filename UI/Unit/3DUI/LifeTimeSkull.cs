using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTimeSkull : MonoBehaviour
{
    private Animator anim;

    public void SetUp()
    {
        anim = GetComponent<Animator>();
    }

    public void Hide()
    {
        Destroy(this.gameObject, 0.01f);
    }

    public void Blink()
    {
        anim.SetBool("Blink", true);
    }

    public void StopBlink()
    {
        anim.SetBool("Blink", false);
    }
}
