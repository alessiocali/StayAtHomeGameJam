using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private float ContagionLevel;
    private bool HasCertification;
    private bool HasToiletPaper;

    public bool ItsMyTurn { get; private set; } = false;

    protected override UpdateTurnResult UpdateTurnInternal()
    {
        return UpdateTurnResult.Completed;

        //ItsMyTurn = true;

        //if (ActionDone)
        //{
        //    ItsMyTurn = false;
        //    return UpdateTurnResult.Completed;
        //}

        //return UpdateTurnResult.Pending;
    }

    public void MoveSomewhere(int x)
    {
        //if (!CanMove())
        //{
        //    return;
        //}

        //Start An Action (move or whatever)
    }

}
