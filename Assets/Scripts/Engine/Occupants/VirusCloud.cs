using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusCloud : ImmovableObject
{
    [Range(0, 1)]
    public float ContagionLevel = 0.3f;

    public int NumberOfTurnsAlive = 3;

    public override UpdateTurnResult UpdateTurn()
    {
        if (NumberOfTurnsAlive == 0)
        {
            Destroy(gameObject);
        }

        NumberOfTurnsAlive--;
        return UpdateTurnResult.Completed;
    }

    public override void OnOtherOccupantCollided(Occupant occupant)
    {
        if (occupant is Player occupantPlayer)
        {
            occupantPlayer.IncreaseContagionLevel(ContagionLevel);
            Destroy(gameObject);
        }
    }

}
