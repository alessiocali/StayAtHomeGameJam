using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    public struct TileIndex
    {
        public TileIndex(int x, int y)
        {
            X = x;
            Y = y;
        }

        int X, Y;
    }

    private TileIndex Index;

    public Vector3 GetOccupantPosition()
    {
        // TODO
        return Vector3.zero;
    }
}
