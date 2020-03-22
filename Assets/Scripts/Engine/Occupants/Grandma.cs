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
        Moving,
        Waiting,
        Sneezing
    };

    private int nextAction = 0;
    //private readonly int enumLength = Enum.GetNames(typeof(GrandmaChoices)).Length;
    private readonly int enumLength = 2;

    protected override UpdateTurnResult UpdateTurnInternal() {

        switch (nextAction) {
            case (int)GrandmaChoices.Moving:
                List<GridTile> tilesList = GameManager.Instance.GridMap.GetCardinalAndWalkableTilesAround(CurrentTileIndex);
                if (tilesList.Count > 0)
                {
                    GridTile nextTile = tilesList[UnityEngine.Random.Range(0, tilesList.Count)];
                    if (!HasStartedMoving)
                        MoveToTile(nextTile.Index);
                }

                break;

            case (int)GrandmaChoices.Waiting:
                break;

            case (int)GrandmaChoices.Sneezing:
                //TODO: spawn
                GameManager.Instance.SpawnOccupantObjectOnTile(sneezePrefab, CurrentTileIndex);
                break;
        
        
        }

        if (IsMoving)
        {
            return UpdateTurnResult.Pending;
        }
        else {
            nextAction = (nextAction + 1) % enumLength;
            return UpdateTurnResult.Completed;
        }
        
    }
}
