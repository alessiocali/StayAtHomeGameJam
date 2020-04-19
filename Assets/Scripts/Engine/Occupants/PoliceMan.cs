using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceMan : Character
{
    public int TurnsBeforeChangingDirection = 4;

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

    public override IEnumerator UpdateTurn()
    {
        yield return MoveToTile(GetTileForward());
        yield return CheckForPlayerAround();
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

    private IEnumerator CheckForPlayerAround()
    {
        List<GridTile> tilesAround = GameManager.Instance.GridMap.GetTilesAround(CurrentTileIndex);
        foreach (GridTile tile in tilesAround)
        {
            Player playerOccupant = tile.GetPlayerOccupant();
            if (playerOccupant)
            {
                yield return StartPunishingPlayer(playerOccupant);
            }
        }
    }

    public IEnumerator StartPunishingPlayer(Player player)
    {
        yield return PlayAnimation("Angry");

        if (player.TestHasCertification())
        {
            player.RemoveCertification();
        }
        else
        {
            player.GoHome();
        }
    }

    private void UpdateDirection()
    {
        TurnsInCurrentDirection++;
        if (TurnsInCurrentDirection > TurnsBeforeChangingDirection)
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
