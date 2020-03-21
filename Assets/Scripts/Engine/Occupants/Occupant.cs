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

    private GridTile.TileIndex TileIndex;

    public GridTile.TileIndex CurrentTileIndex 
    {
        get
        {
            return TileIndex;
        }

        set
        {
            TileIndex = value;
            transform.position = GetCurrentTile().GetOccupantPosition();
        }
    }

    GridTile GetCurrentTile()
    {
        return GameManager.Instance.GridMap.GetTileAt(CurrentTileIndex);
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
