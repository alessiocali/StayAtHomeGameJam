using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmovableObject : Occupant
{
    public override IEnumerator UpdateTurn()
    {
        yield return null;
    }
}
