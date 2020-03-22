﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile:MonoBehaviour
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

    public bool isWalkable { get; set; } = false;
    public bool isBuilding { get; set; } = false;

    public TileIndex Index { get; private set; }
    private Vector3 WordPosition;

    public GameObject TileObject { get; private set; } = null;

    public Occupant Occupant { get; set; } = null;
    
  
    public void InitializeGridTile (GameObject InTileObject, TileIndex InIndex)
    {
        TileObject= InTileObject;
        WordPosition = InTileObject.transform.position + InTileObject.transform.forward * 0.5f + InTileObject.transform.right * 0.5f;
        WordPosition += Vector3.up; 
        Index = InIndex;
    }

    public Vector3 GetOccupantPosition()
    {
        return WordPosition;
    }
}
