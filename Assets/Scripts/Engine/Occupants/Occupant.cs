using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Occupant : MonoBehaviour
{
    private bool IsShuttingDown = false;

    private GridTile.TileIndex TileIndex;

    public GridTile.TileIndex CurrentTileIndex 
    {
        get
        {
            return TileIndex;
        }

        set
        {
            GridTile previousTile = GetCurrentTile();
            previousTile.RemoveOccupant(this);

            TileIndex = value;
            GridTile newTile = GetCurrentTile();
            newTile.AddOccupant(this);
            transform.position = newTile.GetOccupantPosition();
        }
    }

    protected GridTile GetCurrentTile()
    {
        return GameManager.Instance.GridMap.GetTileAt(CurrentTileIndex);
    }

    public abstract IEnumerator UpdateTurn();
    public virtual void OnOtherOccupantCollided(Occupant occupant)
    { }

    private void OnApplicationQuit()
    {
        IsShuttingDown = true;
    }

    private void OnDestroy()
    {
        if (!IsShuttingDown)
        {
            GetCurrentTile().RemoveOccupant(this);
        }
    }
}
