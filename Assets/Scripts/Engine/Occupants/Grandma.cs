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

    public override UpdateTurnResult UpdateTurnInternal() {

        var list = GameManager.Instance.GridMap.GetTilesAround(CurrentTileIndex);
        return UpdateTurnResult.Completed;
    }
}
