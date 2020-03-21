﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile
{
    public struct TileIndex
    {
        public TileIndex (int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
    }

    private TileIndex Index;
    private Vector3 WordPosition;

    public GameObject TileObject { get; private set; } = null;

    public bool IsWalkable { get; private set; } = true;

    public GridTile(GameObject InTileObject, TileIndex InIndex)
    {
        TileObject= InTileObject;
        var GO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        WordPosition = InTileObject.transform.localPosition + InTileObject.transform.forward * 0.5f + InTileObject.transform.right * 0.5f;

        GO.transform.position = WordPosition + Vector3.up;
        GO.transform.localScale /= 2f;
        Index = InIndex;
    }
    
    public Vector3 GetOccupantPosition()
    {
        return WordPosition;
    }
}
