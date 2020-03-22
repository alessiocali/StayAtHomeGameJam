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
    public List<GameObject> MapsList = new List<GameObject>();

    private Dictionary<GridTile.TileIndex, GridTile> Grid = new Dictionary<GridTile.TileIndex, GridTile>();

    public void GenerateGrid ()
    {
        if (MapsList.Count == 0)
        {
            throw new Exception("No map in list");
        }

        int IndexOffsetX = GRID_WIDTH / 2;
        int IndexOffsetY = GRID_HEIGHT / 2;

        int RandomIndex = Random.Range(0, MapsList.Count);

        // Generate a map around the world center
        GameObject Map = Instantiate(MapsList[RandomIndex], new Vector3(0, 0, 0), Quaternion.identity, transform);
        if (!CheckExistsEssentialTiles(Map))
        {
            return;
        }
        
        // Get all information from tile
        foreach (Transform tile in Map.transform)
        {
            Vector3 Location = tile.transform.position;

            int zOffset = 0;
            if (Mathf.Approximately(tile.transform.rotation.eulerAngles.y,90f))
            {
                zOffset++;
            }

            GridTile.TileIndex CurrentIndex = new GridTile.TileIndex(Mathf.CeilToInt(Location.x + IndexOffsetX), Mathf.CeilToInt(Location.z + IndexOffsetY - zOffset));

            GridTile Tile = tile.GetComponent<GridTile>();
            try
            {
                Tile.InitializeGridTile(tile.gameObject, CurrentIndex);
                if (Grid.ContainsKey(CurrentIndex))
                {
                    Debug.Log(Tile.name + " " + Grid[CurrentIndex].name);
                }
                else
                {
                    Grid.Add(CurrentIndex, Tile);
                    tile.name = "Tile_" + CurrentIndex.X + "_" + CurrentIndex.Y;
                }
            }
            catch (NullReferenceException)
            {
                Debug.LogError("Missing GridTile element on prefab");
            }
        }
        Map.transform.parent = transform;
    }
    
    public bool CheckExistsEssentialTiles(GameObject Map)
    {
        List<string> Tags = new List<string>();
        foreach (Transform tile in Map.transform)
        {
            Tags.Add(tile.tag);
        }

        if (Tags.Where(s => s.Equals("House") || s.Equals("SuperMarket")).Count() < 2)
        {
            return false;
        }
        if (Tags.Where(s => s.Equals("Street") || s.Equals("CrossRoad") || s.Contains("Green")).Count() < 3)
        {
            return false;
        }

        return true;
    }
    
    public Vector3 GetFacingRotation (GridTile Current, GridTile Desider)
    {
        if (Current == null || Desider == null)
        {
            return Vector3.zero;
        }
        
        Vector3 Direction = Desider.TileObject.transform.position - Current.TileObject.transform.position;
        return Direction;
    }
    
    public GridTile.TileIndex GetRandomTileIndex(bool isOccupied, bool isWalkable)
    {
        while (true)
        {
            int RandomX = Random.Range(0, GRID_WIDTH);
            int RandomY = Random.Range(0, GRID_HEIGHT);

            GridTile.TileIndex RandomIndex = new GridTile.TileIndex(RandomX, RandomY);
            if (Grid[RandomIndex].HasOccupants() == isOccupied && Grid[RandomIndex].isWalkable == isWalkable)
            {
                return RandomIndex;
            }
        }
    }

    public GridTile.TileIndex GetRandomTileIndex ()
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
