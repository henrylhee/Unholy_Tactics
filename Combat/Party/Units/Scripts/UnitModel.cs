using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

public enum UnitAnimation {MISSING, IDLE, DIE, NORMAL_ATTACK, WALK, TAKE_DAMAGE, SUMMON}
public class UnitModel : MonoBehaviour
{
    public delegate void del();
    [HideInInspector]
    public del AnimationTrigger;
    [HideInInspector]
    public del ActionAnimationStart;
    [HideInInspector]
    public del ActionAnimationEnd;
    [HideInInspector]
    public del DeathFinished;
    [HideInInspector]
    public del Stepped;

    [SerializeField]
    private float rotationTime = 2f;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private Material outlineMaterialSelect;
    [SerializeField]
    private Material outlineMaterialHover;
    [SerializeField]
    private Material outlineMaterialValid;
    [SerializeField]
    private Color outlineColorAlly;
    [SerializeField]
    private Color outlineColorEnemy;

    private bool isActive = false;

    private Transform[] children;
    private LayerMask defaultMask;
    private LayerMask selectedMask;
    private LayerMask hoveredMask;
    private LayerMask validMask;

    public bool turning = false;

    private void Awake()
    {
        defaultMask = LayerMask.NameToLayer("Default");
        selectedMask = LayerMask.NameToLayer("OutlineSelectedUnit");
        hoveredMask = LayerMask.NameToLayer("OutlineHoveredUnit");
        validMask = LayerMask.NameToLayer("OutlineValidUnit");

        children = this.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            child.gameObject.layer = defaultMask;
        }
    }

    public void LookIntoDirection(Vector3 direction)
    {
        if (direction != new Vector3())
        {
            direction.y = 0;
            StartCoroutine(RotateOverTime(Quaternion.LookRotation(direction)));
        }
    }

    private IEnumerator RotateOverTime(Quaternion lookRotation)
    {
        if (isActive)
        {
            yield break;
        }

        turning = true;

        float pastTime = 0;
        isActive = true;
        while (true)
        {
            yield return new WaitForEndOfFrame();

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Mathf.Clamp(pastTime / rotationTime,0,1));
            pastTime += Time.deltaTime;

            if (Quaternion.Angle(transform.rotation, lookRotation) <= 0.01f)
            {
                isActive = false;
                turning = false;
                yield break;
            }
        }
    }

    #region Animation
    public void PlayAnimation(UnitAnimation toPlay)
    {
        anim.SetBool("Die", false);
        anim.SetBool("Walk", false);

        switch(toPlay)
        {
            case UnitAnimation.MISSING:
                Debug.LogError("Error: Tried to play a non existent animation");
                return;
            case UnitAnimation.IDLE:
                break;
            case UnitAnimation.DIE:
                anim.SetBool("Die", true);
                break;
            case UnitAnimation.NORMAL_ATTACK:
                anim.SetBool("Attack", true);
                ActionAnimationStart?.Invoke();
                break;
            case UnitAnimation.WALK:
                anim.SetBool("Walk", true);
                ActionAnimationStart?.Invoke();
                break;
            case UnitAnimation.SUMMON:
                anim.SetBool("Summon", true);
                ActionAnimationStart?.Invoke();
                break;
            case UnitAnimation.TAKE_DAMAGE:
                anim.SetBool("Damaged", true);
                break;
        }
    }

    //called through animation event
    public void PerformAction()
    {
        AnimationTrigger?.Invoke();
    }

    //called through animation event
    public void FinishActionAnimation()
    {
        anim.SetBool("Attack", false);
        ActionAnimationEnd?.Invoke();
    }

    public void FinishSummonAnimation()
    {
        anim.SetBool("Summon", false);
    }

    public void FinishTakeDamage()
    {
        anim.SetBool("Damaged", false);
    }

    public void FinishDeath()
    {
        DeathFinished?.Invoke();
    }

    public void Step()
    {
        Stepped?.Invoke();
    }

    #endregion

    #region OutLine
    public void ActivateOutlineSelectedAlly()
    {
        outlineMaterialSelect.color = outlineColorAlly;
        for(int i = 0;i < transform.childCount;i++)
        {
            transform.GetChild(i).gameObject.layer = selectedMask;
        }
    }

    public void ActivateOutlineSelectedEnemy()
    {
        outlineMaterialSelect.color = outlineColorEnemy;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = selectedMask;
        }
    }

    public void ActivateOutlineAlly()
    {
        if (children[0].gameObject.layer == validMask)
        {
            return;
        }
        else
        {
            outlineMaterialHover.color = outlineColorAlly;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.layer = hoveredMask;
            }
        }
    }

    public void ActivateOutlineEnemy()
    {
        if (children[0].gameObject.layer == validMask)
        {
            return;
        }
        else
        {
            outlineMaterialHover.color = outlineColorEnemy;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.layer = hoveredMask;
            }
        }
    }

    public void DeactivateOutline()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = defaultMask;
        }
    }

    public void ActivateOutlineValidAlly()
    {
        outlineMaterialValid.color = outlineColorAlly;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = validMask;
        }
    }

    public void ActivateOutlineValidEnemy()
    {
        outlineMaterialValid.color = outlineColorEnemy;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = validMask;
        }
    }
    #endregion
}
