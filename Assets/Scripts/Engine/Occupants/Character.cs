using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Character : Occupant
{
    [SerializeField]
    [Range(0,1)]
    public float ContagionLevelOnPlayerBump = 0.3f;

    protected bool IsMoving = false;
    protected bool HasStartedMoving = false;

    public sealed override UpdateTurnResult UpdateTurn()
    {
        UpdateTurnResult updateResult = UpdateTurnInternal();

        if (updateResult == UpdateTurnResult.Completed)
        {
            HasStartedMoving = false;
        }

        return updateResult;
    }

    protected abstract UpdateTurnResult UpdateTurnInternal();

    public override void OnOtherOccupantCollided(Occupant other)
    {
        if (other is Player otherPlayer)
        {
            otherPlayer.IncreaseContagionLevel(ContagionLevelOnPlayerBump);
        }
    }

    protected void MoveToTile(GridTile.TileIndex tileIndex)
    {
        StartCoroutine("MoveCoroutine", tileIndex);
        HasStartedMoving = true;
    }

    private IEnumerator MoveCoroutine(GridTile.TileIndex tileIndex)
    {
        GridTile targetTile = GameManager.Instance.GridMap.GetTileAt(tileIndex);

        IsMoving = true;

        Vector3 originalForward = transform.forward;
        Vector3 targetForward = GameManager.Instance.GridMap.GetFacingRotation(GetCurrentTile(), targetTile);

        float dotWithDesired = Vector3.Dot(originalForward, targetForward);
        if (dotWithDesired < 0.98f)
        {
            const float RotationTime = 0.5f;
            float timeElapsed = 0.0f;
            while (timeElapsed < RotationTime)
            {
                float alpha = Mathf.Clamp(timeElapsed / RotationTime, 0, 1);
                timeElapsed += Time.deltaTime;
                transform.forward = Vector3.LerpUnclamped(originalForward, targetForward, alpha);
                yield return null;
            }
        }

        targetTile.OnOccupantCollided(this);

        if (targetTile.HasCharacterOccupant())
        {
            yield return PlayAnimation("Bump");
        }
        else
        {
            yield return PlayAnimation("Move");
            CurrentTileIndex = tileIndex;
        }

        IsMoving = false;
    }

    private IEnumerator PlayAnimation(string animationTrigger)
    {
        Animator animator = GetComponent<Animator>();
        animator.applyRootMotion = true;
        animator.SetTrigger(animationTrigger);

        bool isPlayingAnimation = true;
        while (isPlayingAnimation)
        {
            yield return null;
            isPlayingAnimation = animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 || animator.IsInTransition(0);
        }

        animator.applyRootMotion = false;
    }

    private void OnAnimatorMove()
    {
        if (IsMoving)
        {
            transform.position = GetComponent<Animator>().rootPosition;
        }
        else
        {
            transform.position = GetCurrentTile().GetOccupantPosition();
        }
    }

    private void Start()
    {
        gameObject.GetComponent<Animator>().applyRootMotion = false;
    }
}
