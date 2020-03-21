using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile
{
    public struct TileIndex
    {
        public TileIndex(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
    }

    public bool isWalkable { get;  set; } = false;

    public TileIndex Index { get; private set; }
    private Vector3 WordPosition;

    public GameObject TileObject { get; private set; } = null;

    public GridTile(GameObject Tile, TileIndex InIndex)
    {
        TileObject= Tile;
        WordPosition = TileObject.transform.position + new Vector3(0, 0.5f, 0);
        Index = InIndex;
    }
    
    public Vector3 GetOccupantPosition()
    {
        return WordPosition;
    }
}
