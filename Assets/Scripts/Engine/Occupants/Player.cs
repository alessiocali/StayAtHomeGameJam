using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private float ContagionLevel;
    private bool HasCertification;
    private bool HasToiletPaper;

    private bool ItsMyTurn = false;

    private void OnEnable()
    {
        InputManager.OnClicked += OnTileClicked;
    }

    private void OnDisable()
    {
        InputManager.OnClicked -= OnTileClicked;
    }

    public void OnTileClicked(string tileName, GridTile.TileIndex tileIndex)
    {
        if (!ItsMyTurn || HasStartedMoving)
        {
            return;
        }

        GridTile targetTile = GameManager.Instance.GridMap.GetTileAt(tileIndex);
        List<GridTile> tilesAround = GameManager.Instance.GridMap.GetCardinalTilesAround(CurrentTileIndex);

        bool isValidTile(GridTile gridTile)
        {
            return  gridTile.isWalkable &&
                    gridTile.Index.X == tileIndex.X &&
                    gridTile.Index.Y == tileIndex.Y;
        }

        bool tileIsValid = tilesAround.Find(isValidTile) != null;
        if (!tileIsValid)
        {
            return;
        }

        MoveToTile(tileIndex);
    }

    public override void OnOtherOccupantCollided(Occupant other)
    {
        base.OnOtherOccupantCollided(other);
        if (other is Character otherCharacter)
        {
            IncreaseContagionLevel(otherCharacter.ContagionLevelOnPlayerBump);
        }
    }

    protected override UpdateTurnResult UpdateTurnInternal()
    {
        if (HasStartedMoving)
        {
            return UpdateMovement();
        }

        ItsMyTurn = true;
        return UpdateTurnResult.Pending;
    }

    private UpdateTurnResult UpdateMovement()
    {
        if (!IsMoving)
        {
            ItsMyTurn = false;
            return UpdateTurnResult.Completed;
        }

        return UpdateTurnResult.Pending;
    }

    public void IncreaseContagionLevel(float amount)
    {
        ContagionLevel += amount;
        if (ContagionLevel >= 1)
        {
            GameManager.Instance.GameOver();
        }
    }
}
