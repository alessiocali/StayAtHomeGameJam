using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandma : Character
{
    protected override UpdateTurnResult UpdateTurnInternal() {
        List<GridTile> tilesList = GameManager.Instance.GridMap.GetTilesAround(CurrentTileIndex);


        return UpdateTurnResult.Completed;
    }
}
