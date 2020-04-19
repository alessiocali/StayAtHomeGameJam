using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandma : Character
{
    [SerializeField]
    private GameObject sneezePrefab;

    private enum GrandmaChoices
    {
        Moving = 0,
        Waiting = 1,
        Sneezing = 2,
        COUNT = 3
    };

    private GrandmaChoices nextAction = GrandmaChoices.Moving;

    public override IEnumerator UpdateTurn()
    {
        switch (nextAction)
        {
            case GrandmaChoices.Moving:
                List<GridTile> tilesList = GameManager.Instance.GridMap.GetCardinalAndWalkableTilesAround(CurrentTileIndex);
                
                if (tilesList.Count > 0)
                {
                    GridTile nextTile = tilesList[UnityEngine.Random.Range(0, tilesList.Count)];
                    yield return MoveToTile(nextTile.Index);
                }

                break;

            case GrandmaChoices.Waiting:
                break;

            case GrandmaChoices.Sneezing:
                SpawnVirusClouds();
                AudioManager.Instance.PlayGrandmaCough();
                break;
        }

        SetNextChoice();
    }

    private void SetNextChoice()
    {
        int nextActionIdx = (int)nextAction;
        const int actionCount = (int)GrandmaChoices.COUNT;

        nextAction = (GrandmaChoices)((nextActionIdx + 1) % actionCount);
    }

    private void SpawnVirusClouds()
    {
        if (sneezePrefab == null)
        {
            return;
        }

        foreach (GridTile tile in GameManager.Instance.GridMap.GetTilesAround(CurrentTileIndex))
        {
            if (!tile.isWalkable || tile.HasOccupants())
            {
                continue;
            }

            GameManager.Instance.SpawnOccupantObjectOnTile(sneezePrefab, tile.Index);
        }
    }
}
