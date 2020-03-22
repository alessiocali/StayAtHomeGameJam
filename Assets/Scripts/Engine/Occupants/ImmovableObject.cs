using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmovableObject : Occupant
{
    public override UpdateTurnResult UpdateTurn()
    {
        return UpdateTurnResult.Completed;
    }
}
