using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class AIUnit : Unit
{
    [Header("AI Unit")]
    [SerializeField]
    private bool reckless;
    [SerializeField]
    protected AiActions aiActions;

    [HideInInspector]
    public UnityEvent<AIUnit> finishedFirstMovement;
    [HideInInspector]
    public UnityEvent<AIUnit> finishedSecondMovement;
    [HideInInspector]
    public UnityEvent<AIUnit> finishedActing; 

    private int moveCounter = 0;
    private Vector3 debugMoveTarget = new Vector3();

    public override void Setup(Inputs inputs)
    {
        base.Setup(inputs);

        SetUpAIActions();
        Deselect();
    }


    #region AI Decisions
    public void FirstMovement(List<Unit> allies, List<Unit> enemies)
    {
        moveCounter = 1;
        OffensiveMovement(allies, enemies);
    }

    public void SecondMovement(List<Unit> allies, List<Unit> enemies)
    {
        moveCounter = 2;
        DefensiveMovement(allies, enemies);
    }

    private void DefensiveMovement(List<Unit> allies, List<Unit> enemies)
    {
        if (reckless)
        {
            OnFinishMovement();
            return;
        }

        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(0.5f);
            InterestData bestInterestPoint = InterestMatrix.instance.GetBestSaveArea(startingPosition, aiActions.GetMovementRadius(this), this, enemies);

            GameObject p = new GameObject();
            PointOfInterest point = p.AddComponent<PointOfInterest>();
            point.transform.position = transform.position;
            Destroy(point.gameObject, 1);

            if(bestInterestPoint == null || bestInterestPoint.point == null || point.GetCoverCount(this, enemies) >= bestInterestPoint.coverCount)
            {
                OnFinishMovement();
                yield break; 
            }
            aiActions.Move(bestInterestPoint.point.transform.position, new List<AiActionInformation>());

            debugMoveTarget = bestInterestPoint.point.transform.position;
        }
    }

    private void OffensiveMovement(List<Unit> allies, List<Unit> enemies)
    {
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(0.5f);
            Vector3 target = aiActions.GetClosestUnit(enemies, this).transform.position;
            aiActions.Move(target, aiActions.UntilInAttackRange(enemies));

            debugMoveTarget = target;
        }
    }

    public void Act(List<Unit> allies, List<Unit> enemies)
    {
        AiActionInformation info = aiActions.GetHighestDamageAttack(enemies);

        aiActions.Act(info);
    }

    #endregion

    private void OnFinishMovement()
    {
        debugMoveTarget = new Vector3();
        switch (moveCounter)
        {
            case 1:
                moveCounter = 0;
                finishedFirstMovement?.Invoke(this);
                break;
            case 2:
                moveCounter = 0;
                finishedSecondMovement?.Invoke(this);
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if(moveCounter != 0)
        {
            Gizmos.DrawWireSphere(startingPosition, aiActions.GetMovementRadius(this));
        }

        if(debugMoveTarget != new Vector3())
        {
            Gizmos.color = UnityEngine.Color.green;
            Gizmos.DrawSphere(debugMoveTarget, 0.5f);
        }
    }

    public override void Select()
    {
        base.Select();
        model.ActivateOutlineSelectedEnemy();
    }

    public override void TurnStart()
    {
        base.TurnStart();
        aiActions.TurnStart();
    }

    public override void TurnEnd()
    {
        base.TurnEnd();
        aiActions.TurnEnd();
    }

    private void SetUpAIActions()
    {
        aiActions.SetUp(this);
        aiActions.FinishedResolving.AddListener(OnFinishResolving);

        aiActions.finishedMoving = OnFinishMovement;
        aiActions.finishedActing = () => finishedActing?.Invoke(this);
    }

    public override void TakeDamage(int damage, bool crit)
    {
        base.TakeDamage(damage, crit);
        if(damage > 0)
        {
            unitVFX.PlayEnemyBlood();
        }
    }
}
