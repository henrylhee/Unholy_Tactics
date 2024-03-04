using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "ExperimentalMoveAction", menuName = "Actions/Experimental Move", order = 1)]
public class ExperimentalMove : Action
{
    [Header("Move")]
    [SerializeField]
    private float range;
    [SerializeField]
    private ActionSound moveLoop;

    private AudioSource activeLoop;
    private Vector3 targetPosition;

    public override void SetUp(Unit caster)
    {
        base.SetUp(caster);
        _range = range;
    }
    
    #region Perform
    protected override bool IsLegal(Unit caster)
    {
        Vector3 target = GetTargetPosition(caster, true);
        if(caster is PlayerUnit)
        {
            if (!((PlayerUnit)caster).WithinMovementRange(target))
            {
               // return false;
            }
        }

        return target != new Vector3();
    }

    protected override bool Perform(Unit caster)
    {
        PlayActionSound(moveLoop, caster);
        caster.model.PlayAnimation(UnitAnimation.WALK);

        targetPosition = GetTargetPosition(caster, true);

        caster.Move(targetPosition);

        if(caster is PlayerUnit)
        {
            ((PlayerUnit)caster).arrow.ClearPath();
        }
       
        return true;
    }

    public override bool ResolveUpdate(Unit caster)
    {
        if(Vector3.Distance(caster.startingPosition, caster.transform.position) > GetMovementRange(caster))
        {
            caster.transform.position = caster.startingPosition + (caster.transform.position - caster.startingPosition).normalized * GetMovementRange(caster) * 0.999f;
            caster.Move(caster.transform.position);
        }
        caster.LookIntoMovingDirection();
        if (!caster.IsMoving()) 
        {
            caster.Move(caster.transform.position);
            FinishResolving(caster);
            base.usedUp = false;

            if(activeLoop != null)
            {
                Destroy(activeLoop.gameObject);
            }

            return false;
        }
        return true;
    }
    #endregion

    #region Preview
    protected override void SetupPreview(PlayerUnit caster)
    {
        caster.actionRange.GenerateMoveRange(GetMovementRange(caster), new Vector3(caster.startingPosition.x, 0, caster.startingPosition.z));
        previewSetuped = true;
    }

    protected override void UpdatePreview(PlayerUnit caster)
    {
        Vector3 target = GetTargetPosition(caster, true);
        if (target == new Vector3() || !caster.WithinMovementRange(target))
        {
            caster.arrow.ClearPath();
        }
        else
        {
            caster.arrow.CalculatePath(caster.transform.position, caster.startingPosition, GetTargetPosition(caster, true), GetMovementRange(caster));
            caster.arrow.RenderPath(GetMovementRange(caster));
        }
    }

    public override void ShutDownPreview(PlayerUnit caster)
    {
        caster.arrow.ClearPath();
        caster.actionRange.ClearMoveRange();

        base.ShutDownPreview(caster);
    }

    #endregion

    #region Other
    public override void Abort(Unit caster)
    {
        base.Abort(caster);

        if (activeLoop != null)
        {
            Destroy(activeLoop.gameObject);
        }
    }

    private void PlayActionSound(ActionSound actionSound, Unit caster)
    {
        if(activeLoop != null)
        {
            Destroy(activeLoop.gameObject);
        }

        GameObject instance = new GameObject();
        instance.gameObject.name = name + " Sound";
        instance.transform.position = caster.transform.position;

        activeLoop = instance.AddComponent<AudioSource>();
        activeLoop.volume = actionSound.volume;
        activeLoop.clip = actionSound.audioClip;
        activeLoop.outputAudioMixerGroup = actionSound.group;
        activeLoop.loop = true;
        activeLoop.Play();
    }

    public float GetMovementRange(Unit caster)
    {
        return Mathf.Clamp(caster.stats.buffMoveRange * _range, 0, caster.stats.buffMoveRange * _range);
    }

    protected override Vector3 GetTargetPosition(Unit caster, bool fromStartPoint = false)
    {
        if (aITargetPosition != new Vector3())
        {
            return aITargetPosition;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, float.PositiveInfinity, LayerMask.GetMask("MoveArea")))
        {
            var distance = 0f;

            if (fromStartPoint)
            {
                distance = Vector2.Distance(new Vector2(hit.point.x, hit.point.z),
                new Vector2(caster.startingPosition.x, caster.startingPosition.z));
            }
            else
            {
                distance = Vector2.Distance(new Vector2(hit.point.x, hit.point.z),
               new Vector2(caster.transform.position.x, caster.transform.position.z));
            }
            if (distance <= GetMovementRange(caster) && hit.transform.tag != "Obstacle" && !EventSystem.current.IsPointerOverGameObject())
            {
                return hit.point;
            }
        }

        return new Vector3();
    }
    #endregion
}
