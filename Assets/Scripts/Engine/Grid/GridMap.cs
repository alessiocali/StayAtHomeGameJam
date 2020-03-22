using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class GridMap : MonoBehaviour
{
    public readonly int GRID_WIDTH = 8;
    public readonly int GRID_HEIGHT = 8;

    [SerializeField]
    // TODO: add a list of possible tiles in the map
    public List<GameObject> NotWalkableCube = new List<GameObject>();
    public List<GameObject> WalkableCube = new List<GameObject>();


    [SerializeField]
    public float GridRotation = -45f;
    
    private Dictionary<GridTile.TileIndex, GridTile> Grid = new Dictionary<GridTile.TileIndex, GridTile>();

    public void GenerateGrid()
    {
        if (WalkableCube.Count == 0 || NotWalkableCube.Count == 0 )
        {
            return;
        }
        
        int IndexOffsetX = GRID_WIDTH / 2;
        int IndexOffsetY = GRID_HEIGHT / 2;
        
        // Generate a map around a center
        GameObject Originator = Instantiate(NotWalkableCube[0], new Vector3(0,0,0), Quaternion.Euler(0, GridRotation, 0), transform);
        for (int X = -IndexOffsetX; X < IndexOffsetX; ++X)
        {

            for (int Y = -IndexOffsetY; Y < IndexOffsetY; ++Y)
            {
                Vector3 NextVector = Originator.transform.forward * X + Originator.transform.right * Y;
                GameObject Current3Cube = Instantiate(NotWalkableCube[0], NextVector, Quaternion.Euler(0, GridRotation, 0), transform);
                Current3Cube.name = "Tile_" + (X + IndexOffsetX) + "_" + (Y + IndexOffsetY);
                
                GridTile.TileIndex CurrentIndex = new GridTile.TileIndex(X+IndexOffsetX, Y+IndexOffsetY);
                //GridTile Tile = new GridTile(Current3Cube, CurrentIndex);
                //GridTile Tile = Current3Cube.AddComponent<GridTile>();
                GridTile Tile = Current3Cube.GetComponent<GridTile>();
                try
                {
                    Tile.InitializeGridTile(Current3Cube, CurrentIndex);
                    Tile.isWalkable = true;
                    Grid.Add(CurrentIndex, Tile);
                }
                catch (NullReferenceException)
                {
                    Debug.LogError("Missing GridTile element on prefab");
                }
            }
        }
        Destroy(Originator);
    }

    public Vector3 GetFacingRotation (GridTile Current, GridTile Desider)
    {
        if (Current == null || Desider == null)
        {
            return Vector3.zero;
        }

        Vector3 Direction = Vector3.Normalize(Desider.TileObject.transform.position - Current.TileObject.transform.position);
        return Direction;
    }
    
    public GridTile.TileIndex GetRandomTileIndex()
    {
        int RandomX = Random.Range(0, GRID_WIDTH);
        int RandomY = Random.Range(0, GRID_HEIGHT);

        return new GridTile.TileIndex(RandomX, RandomY);
    }

    public GridTile GetTileAt(GridTile.TileIndex index)
    {
        return Grid.ContainsKey(index) ? Grid[index] : null;
    }

    public List<GridTile> GetTilesAround(GridTile.TileIndex InIndex)
    {
        List<GridTile> Tiles = new List<GridTile>();

        if (!Grid.ContainsKey(InIndex))
        {
            return Tiles;
        }
  
        for (int X = InIndex.X - 1; X <= InIndex.X + 1; ++X)
        {
            for (int Y = InIndex.Y - 1; Y <= InIndex.Y + 1; ++Y)
            {
                if (InIndex.X == X && InIndex.Y == Y)
                {
                    continue;
                }

                GridTile.TileIndex CurrentIndex = new GridTile.TileIndex(X, Y);
                if (Grid.ContainsKey(CurrentIndex))
                {
                    Tiles.Add(Grid[CurrentIndex]);
                }
            }
        }
        return Tiles;
    }


    public List<GridTile> GetWalkableTilesAround(GridTile.TileIndex InIndex)
    {
        List<GridTile> Tiles = GetTilesAround(InIndex);

        return Tiles.Where(x => x.isWalkable == true).ToList();
    }

    public List<GridTile> GetDiagonalTilesAround(GridTile.TileIndex InIndex)
    {
        List<GridTile> Tiles = new List<GridTile>();

        if (!Grid.ContainsKey(InIndex))
        {
            return Tiles;
        }

        for (int X = InIndex.X - 1; X <= InIndex.X + 1; X+=2)
        {
            for (int Y = InIndex.Y - 1; Y <= InIndex.Y + 1; Y+=2)
            {
                if (InIndex.X == X && InIndex.Y == Y)
                {
                    continue;
                }

                GridTile.TileIndex CurrentIndex = new GridTile.TileIndex(X, Y);
                if (Grid.ContainsKey(CurrentIndex))
                {
                    Tiles.Add(Grid[CurrentIndex]);
                }
            }
        }
        return Tiles;
    }

    public List<GridTile> GetCardinalTilesAround(GridTile.TileIndex InIndex)
    {
        List<GridTile> Tiles = new List<GridTile>();

        if (!Grid.ContainsKey(InIndex))
        {
            return Tiles;
        }

        for (int X = InIndex.X - 1; X <= InIndex.X + 1; X += 2)
        {
            GridTile.TileIndex CurrentIndex = new GridTile.TileIndex(X, InIndex.Y);
            if (Grid.ContainsKey(CurrentIndex))
            {
                Tiles.Add(Grid[CurrentIndex]);
            }
        }
        for (int Y = InIndex.Y - 1; Y <= InIndex.Y + 1; Y += 2)
        {
            GridTile.TileIndex CurrentIndex = new GridTile.TileIndex(InIndex.X, Y);
            if (Grid.ContainsKey(CurrentIndex))
            {
                Tiles.Add(Grid[CurrentIndex]);
            }
        }

        return Tiles;
    }

    public List<GridTile> GetCardinalAndWalkableTilesAround(GridTile.TileIndex InIndex)
    {
        List<GridTile> Tiles = GetCardinalTilesAround(InIndex);

        return Tiles.Where(x => x.isWalkable == true).ToList();
    }
}
