using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    //private float ContagionLevel;
    private int ContagionLevel = 0;

    private bool HasCertification;
    private bool HasToiletPaper;

    private bool ItsMyTurn = false;

    public bool TestHasCertification()
    {
        return HasCertification;
    }

    public void RemoveCertification()
    {
        HasCertification = false;
        GameManager.Instance.SetCertificationHUDActive(false);
    }

    public void AddCertification()
    {
        HasCertification = true;
        GameManager.Instance.SetCertificationHUDActive(true);
    }

    public void GoHome()
    {
        CurrentTileIndex = GameManager.Instance.GridMap.GetHomeTileIndex();
    }

    private void OnEnable()
    {
        InputManager.OnClicked += OnTileClicked;
    }

    private void OnDisable()
    {
        InputManager.OnClicked -= OnTileClicked;
    }

    public bool IsWaitingForInput()
    {
        return ItsMyTurn && !HasStartedMoving;
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
            return  (gridTile.isWalkable || gridTile.isBuilding) &&
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
        if (other is Character)
        {
            Character otherCharacter = other as Character;
            if (otherCharacter.IsInfected)
            {
                IncreaseContagionLevel();
            }
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
            CheckBuildingEffect();
            ItsMyTurn = false;
            return UpdateTurnResult.Completed;
        }

        return UpdateTurnResult.Pending;
    }

    private void CheckBuildingEffect()
    {
        GridTile currentTile = GetCurrentTile();
        if (!currentTile.isBuilding)
        {
            return;
        }

        if (currentTile.CompareTag("House") && HasToiletPaper)
        {
            GameManager.Instance.GameWon();
            return;
        }

        if (currentTile.CompareTag("SuperMarket"))
        {
            HasToiletPaper = true;
            GameManager.Instance.SetToiletPaperHUDActive(true);
            return;
        }
    }

    public void IncreaseContagionLevel()
    {
        ContagionLevel ++;
        GameManager.Instance.SetMaskHUDActive(false, ContagionLevel);

        if (ContagionLevel >= 3)
        {
            GameManager.Instance.GameOver();
        }
    }
}
