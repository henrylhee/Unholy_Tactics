using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[System.Serializable]
public class AiActions : Actions
{
    public delegate void del();

    [HideInInspector]
    public del finishedMoving;
    [HideInInspector]
    public del finishedActing;

    List<Attack> attacks = new List<Attack>();

    public virtual void SetUp(Unit caster)
    {
        base.Setup(caster);

        foreach(Action a in availableActions)
        {
            caster.model.ActionAnimationEnd += () => a.OnAnimationEnd(caster);
            caster.model.AnimationTrigger += () => a.OnAnimationTrigger(caster);

            if(a is Attack)
            {
                attacks.Add((Attack)a);
            }
        }
    }

    public void Move(Vector3 position, List<AiActionInformation> stopConditions)
    {
        caster.StartCoroutine(move());
        IEnumerator move()
        {
            moveAction.SetTargetPosition(position);
            moveAction.Perform_(caster);

            yield return new WaitForSeconds(0.1f);

            bool breakFlag = false;
            while (moveAction.IsResolving && !breakFlag)
            {
                moveAction.ResolveUpdate(caster);
                caster.LookIntoMovingDirection();

                foreach (AiActionInformation ai in stopConditions)
                {
                    if (ai.CanPerform(caster))
                    {
                        moveAction.Abort(caster);
                        breakFlag = true;
                    }
                }
               
                yield return null;
            }

            caster.Move(caster.transform.position);
            finishedMoving?.Invoke();
        }
    }

    public void Act(AiActionInformation info)
    {
        if(info == null || info == new AiActionInformation() || info.action == null)
        {
            finishedActing?.Invoke();
            return;
        }


        caster.StartCoroutine(act());
        IEnumerator act()
        {
            info.action.SetTargetPosition(info.position);
            info.action.SetTargetUnit(info.unit);

            while (caster.model.turning)
            {
                yield return null;
            }

            info.action.Perform_(caster);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            bool breakFlag = false;
            while (info.action.IsResolving && !breakFlag)
            {
                info.action.ResolveUpdate(caster);
                yield return null;
            }

            finishedActing?.Invoke();
        }
    }

    public AiActionInformation GetHighestDamageAttack(List<Unit> targets)
    {
        AiActionInformation result = new AiActionInformation();

        foreach(Unit target in targets)
        {
            foreach (Attack a in attacks)
            {
                AiActionInformation info = new AiActionInformation();
                info.action = a;
                info.unit = target;
                if (a.GetAverageDamage(caster, target) >= result.GetAverageDamage(caster) && info.CanPerform(caster))
                {
                    result = info;
                }
            }
        }
        
        if(result == new AiActionInformation())
        {
            return null;
        }
        return result;
    }

    public List<AiActionInformation> UntilInAttackRange(List<Unit> targets = null, List<Vector3> positions = null)
    {
        List<AiActionInformation> result = new List<AiActionInformation>();

        foreach(Attack a in attacks)
        {
            foreach(Unit target in targets)
            {
                if(positions != null)
                {
                    foreach (Vector3 position in positions)
                    {
                        AiActionInformation info_ = new AiActionInformation();

                        info_.action = a;
                        info_.unit = target;
                        info_.position = position;
                        result.Add(info_);
                    }

                }

                AiActionInformation info = new AiActionInformation();

                info.action = a;
                info.unit = target;
                result.Add(info);
            }
        }

        return result;
    }

    public Unit GetClosestUnit(List<Unit> units, Unit caster)
    {
        float distance = Mathf.Infinity;
        Unit target = null;

        foreach(Unit u in units)
        {
            if(target == null)
            {
                target = u;
                distance = Vector3.Distance(caster.transform.position, target.transform.position);
                continue;
            }

            if(Vector3.Distance(caster.transform.position, target.transform.position) < distance)
            {
                target = u;
                distance = Vector3.Distance(caster.transform.position, target.transform.position);
            }
        }

        return target;
    }

    public float GetMovementRadius(Unit caster)
    {
        return moveAction.GetMovementRange(caster);
    }
}

public class AiActionInformation
{
    public Action action;
    public Vector3 position;
    public Unit unit;

    public bool CanPerform(Unit caster)
    {
        if(action == null)
        {
            return false;
        }

        action.SetTargetUnit(unit);
        action.SetTargetPosition(position);

        bool result = action.CanPerform(caster);

        action.ClearTargetPosition();
        action.ClearTargetUnit();

        return result;
    }

    public float GetAverageDamage(Unit caster)
    {
        if(action == null || action is not Attack)
        {
            return 0;
        }

        return ((Attack)action).GetAverageDamage(caster, unit);
    }
}
