using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Character : Occupant
{
    public bool IsInfected = true;

    public override void OnOtherOccupantCollided(Occupant other)
    {
        if (IsInfected && other is Player otherPlayer)
        {
            otherPlayer.IncreaseContagionLevel();
        }
    }

    protected IEnumerator MoveToTile(GridTile.TileIndex tileIndex)
    {
        GridTile targetTile = GameManager.Instance.GridMap.GetTileAt(tileIndex);
        yield return RotateTowardsTile(targetTile);

        bool MoveSucceded = true;

        if (targetTile.HasCharacterOccupant())
        {
            AudioManager.Instance.PlayCharacterBump();
            yield return PlayAnimation("Bump");
            MoveSucceded = false;
        }
        else if (targetTile.isBuilding)
        {
            yield return PlayAnimation("GetInBuilding");
        }
        else
        {
            yield return PlayAnimation("Move");
        }

        targetTile.OnOccupantCollided(this);

        if (MoveSucceded)
        {
            CurrentTileIndex = tileIndex;
        }
    }

    private IEnumerator RotateTowardsTile(GridTile targetTile)
    {
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
                yield return new WaitForFixedUpdate();
            }
        }
    }

    protected IEnumerator PlayAnimation(string animationTrigger)
    {
        Animator animator = GetComponent<Animator>();
        animator.applyRootMotion = true;
        animator.SetTrigger(animationTrigger);

        do
        {
            yield return new WaitForFixedUpdate();
        }
        while (IsPlayingAnimation());

        animator.applyRootMotion = false;
    }

    private void OnAnimatorMove()
    {
        if (GetComponent<Animator>().applyRootMotion)
        {
            transform.position = GetComponent<Animator>().rootPosition;
        }
        else
        {
            transform.position = GetCurrentTile().GetOccupantPosition();
        }
    }

    private bool IsPlayingAnimation()
    {
        Animator animator = GetComponent<Animator>();
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 || animator.IsInTransition(0);
    }

    private void Start()
    {
        gameObject.GetComponent<Animator>().applyRootMotion = false;
    }
}
