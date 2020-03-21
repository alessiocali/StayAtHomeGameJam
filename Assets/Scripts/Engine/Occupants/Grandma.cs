using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandma : Character 
{
    private enum GrandmaChoices{
        Moving,
        Waiting,
        Sneezing
    };

    protected override UpdateTurnResult UpdateTurnInternal() {
        List<GridTile> tilesList = GameManager.Instance.GridMap.GetTilesAround(CurrentTileIndex);
        return UpdateTurnResult.Completed;
    }
}
