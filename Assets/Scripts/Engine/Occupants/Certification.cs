using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Certification : ImmovableObject
{
    public override void OnOtherOccupantCollided(Occupant occupant)
    {
        if (occupant is Player)
        {
            (occupant as Player).AddCertification();
            Destroy(gameObject);
        }
    }
}
