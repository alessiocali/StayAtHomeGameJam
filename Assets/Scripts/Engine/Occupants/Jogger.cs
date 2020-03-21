using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jogger : Character
{
    public int TurnsBeforeChangingDirection = 4;
    public GameObject VirusPrefab = null;

    private struct DesiredDirection
    {
        public DesiredDirection(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X, Y;
    }

    private readonly List<DesiredDirection> Directions = new List<DesiredDirection>();
    private int CurrentDirectionIndex;
    private int TurnsInCurrentDirection = 0;

    private DesiredDirection CurrentDirection { get { return Directions[CurrentDirectionIndex]; } }

    protected override UpdateTurnResult UpdateTurnInternal()
    {
        if (!HasStartedMoving)
        {
            UpdateCurrentDirection();

            GridTile.TileIndex nextTile = GetNextTileIndex();
            if (nextTile.X != CurrentTileIndex.X || nextTile.Y != CurrentTileIndex.Y)
            {
                SpawnVirusOnCurrentTile();
            }

            MoveToTile(nextTile);
        }

        return IsMoving ? UpdateTurnResult.Pending : UpdateTurnResult.Completed;
    }

    private void SpawnVirusOnCurrentTile()
    {
        if (VirusPrefab == null)
        {
            return;
        }

        GameManager.Instance.SpawnOccupantObjectOnTile(VirusPrefab, CurrentTileIndex);
    }

    private void UpdateCurrentDirection()
    {
        TurnsInCurrentDirection++;
        if (TurnsInCurrentDirection > TurnsBeforeChangingDirection)
        {
            SetNextDirection();
        }
    }

    private void SetNextDirection()
    {
        TurnsInCurrentDirection = 0;
        CurrentDirectionIndex = (CurrentDirectionIndex + 1) % Directions.Count;
    }

    private GridTile GetTileFacingCurrentDirection()
    {
        GridTile.TileIndex indexFacing = new GridTile.TileIndex(CurrentTileIndex.X + CurrentDirection.X, CurrentTileIndex.Y + CurrentDirection.Y);
        return GameManager.Instance.GridMap.GetTileAt(indexFacing);
    }

    private GridTile.TileIndex GetNextTileIndex()
    {
        int triesLeft = 4;
        while (triesLeft > 0)
        {
            GridTile nextTile = GetTileFacingCurrentDirection();
            if (nextTile != null)
            {
                return nextTile.Index;
            }

            triesLeft--;
            SetNextDirection();
        }

        return CurrentTileIndex;
    }

    private void InitDesiredDirections()
    {
        Directions.Add(new DesiredDirection(0, 1));
        Directions.Add(new DesiredDirection(1, 0));
        Directions.Add(new DesiredDirection(0, -1));
        Directions.Add(new DesiredDirection(-1, 0));

        CurrentDirectionIndex = 0;
    }

    private void Start()
    {
        InitDesiredDirections();
    }
}
