using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Occupant : MonoBehaviour
{
    public enum UpdateTurnResult
    {
        Completed, 
        Pending
    }

    GridTile.TileIndex CurrentTileIndex;

    GridTile GetCurrentTile()
    {
        return null;
    }

    public abstract UpdateTurnResult UpdateTurn();
    public virtual void OnOtherOccupantCollided(Occupant occupant)
    { }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
