using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridMap : MonoBehaviour
{
    public readonly int GRID_WIDTH = 8;
    public readonly int GRID_HEIGHT = 8;

    [SerializeField]
    // TODO: add a list of possible tiles in the map
    public GameObject Cube = null;

    [SerializeField]
    public float GridRotation = -45f;
    
    private Dictionary<GridTile.TileIndex, GridTile> Grid = new Dictionary<GridTile.TileIndex, GridTile>();

    public void GenerateGrid()
    {
        if (!Cube)
        {
            return;
        }
        
        int IndexOffsetX = GRID_WIDTH / 2;
        int IndexOffsetY = GRID_HEIGHT / 2;
        
        // Generate a map around an 
        GameObject Originator = Instantiate(Cube, new Vector3(0,0,0), Quaternion.Euler(0, GridRotation, 0), transform);
        for (int X = -IndexOffsetX; X < IndexOffsetX; ++X)
        {

            for (int Y = -IndexOffsetY; Y < IndexOffsetY; ++Y)
            {
                Vector3 NextVector = Originator.transform.forward * X + Originator.transform.right * Y;
                GameObject Current3Cube = Instantiate(Cube, NextVector, Quaternion.Euler(0, GridRotation, 0), transform);
                Current3Cube.name = "Tile_" + (X + IndexOffsetX) + "_" + (Y + IndexOffsetY);
                
                GridTile.TileIndex CurrentIndex = new GridTile.TileIndex(X+IndexOffsetX, Y+IndexOffsetY);
                GridTile Tile = new GridTile(Current3Cube, CurrentIndex);

                Grid.Add(CurrentIndex, Tile);
            }
        }
        GameObject.Destroy(Originator);
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
}
