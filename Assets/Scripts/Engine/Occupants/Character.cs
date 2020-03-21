using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : Occupant
{
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

    protected void MoveToTile(GridTile.TileIndex tileIndex)
    {
        StartCoroutine("MoveCoroutine", tileIndex);
        HasStartedMoving = true;
    }

    private IEnumerator MoveCoroutine(GridTile.TileIndex tileIndex)
    {
        IsMoving = true;
        yield return new WaitForSeconds(2.0f);

        CurrentTileIndex = tileIndex;
        IsMoving = false;
    }

}
