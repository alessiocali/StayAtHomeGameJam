using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceMan : Character
{
    public int TurnsBeforeChangingDirection = 4;

    private bool HasStartedPunishingPlayer = false;
    private bool IsPunishingPlayer = false;

    private struct DesiredDirection
    {
        public DesiredDirection(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X, Y;
    }

    private DesiredDirection CurrentDirection;
    private int TurnsInCurrentDirection = 0;

    protected override UpdateTurnResult UpdateTurnInternal()
    {
        // Forgive me for this madness, I have no time to fix the implementation right now.
        // @agcali's fault.

        if (IsPunishingPlayer)
        {
            return UpdateTurnResult.Pending;
        }

        if (!HasStartedMoving)
        {
            MoveToTile(GetTileForward());
        }

        if (!IsMoving)
        {
            if (!HasStartedPunishingPlayer)
            {
                HasStartedPunishingPlayer = true;
                CheckForPlayerAround(); 
            }

            if (!IsPunishingPlayer)
            {
                HasStartedPunishingPlayer = false;
                return UpdateTurnResult.Completed;
            }
            
            return UpdateTurnResult.Pending;
        }

        return UpdateTurnResult.Pending;
    }

    private GridTile.TileIndex GetTileForward()
    {
        UpdateDirection();

        int tries = 2;
        while (tries > 0)
        {
            GridTile.TileIndex nextIndex = new GridTile.TileIndex(CurrentTileIndex.X + CurrentDirection.X, CurrentTileIndex.Y + CurrentDirection.Y);
            GridTile tileForward = GameManager.Instance.GridMap.GetTileAt(nextIndex);

            if (tileForward && tileForward.isWalkable)
            {
                return nextIndex;
            }

            FlipCurrentDirection();
        }

        return CurrentTileIndex;
    }

    private void CheckForPlayerAround()
    {
        List<GridTile> tilesAround = GameManager.Instance.GridMap.GetTilesAround(CurrentTileIndex);
        foreach (GridTile tile in tilesAround)
        {
            Player playerOccupant = tile.GetPlayerOccupant();
            if (playerOccupant)
            {
                StartCoroutine("StartPunishingPlayer", playerOccupant);
                return;
            }
        }
    }

    public IEnumerator StartPunishingPlayer(Player player)
    {
        IsPunishingPlayer = true;
        IsPerformingCustomAction = true;

        yield return PlayAnimation("Angry");

        if (player.TestHasCertification())
        {
            player.RemoveCertification();
        }
        else
        {
            player.GoHome();
        }
        
        IsPunishingPlayer = false;
        IsPerformingCustomAction = false;
    }

    private void UpdateDirection()
    {
        TurnsInCurrentDirection++;
        if (TurnsInCurrentDirection >= TurnsBeforeChangingDirection)
        {
            TurnsInCurrentDirection = 0;
            FlipCurrentDirection();
        }
    }

    private void FlipCurrentDirection()
    {
        CurrentDirection.X = -CurrentDirection.X;
        CurrentDirection.Y = -CurrentDirection.Y;
    }

    private void Start()
    {
        List<DesiredDirection> possibleDirections = new List<DesiredDirection>
        {
            new DesiredDirection(0, 1),
            new DesiredDirection(1, 0),
            new DesiredDirection(0, -1),
            new DesiredDirection(-1, 0)
        };

        CurrentDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
    }
}
