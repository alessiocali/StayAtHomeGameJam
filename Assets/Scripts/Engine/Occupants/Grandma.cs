using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandma : Character
{
    public override UpdateTurnResult UpdateTurn() {
        List<GridTile> tilesList = GameManager.Instance.GridMap.GetTilesAround(CurrentTileIndex);


        return UpdateTurnResult.Completed;
    }
}
