using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private float ContagionLevel;
    private bool HasCertification;
    private bool HasToiletPaper;

    public override void OnOtherOccupantCollided(Occupant other)
    {
        base.OnOtherOccupantCollided(other);
        if (other is Character otherCharacter)
        {
            IncreaseContagionLevel(otherCharacter.ContagionLevelOnPlayerBump);
        }
    }

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

    public void IncreaseContagionLevel(float amount)
    {
        ContagionLevel += amount;
        if (ContagionLevel >= 1)
        {
            GameManager.Instance.GameOver();
        }
    }
}
