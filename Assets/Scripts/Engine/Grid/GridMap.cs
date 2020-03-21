using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // Generate a map around the origin (0,0)
        for (int X = -IndexOffsetX; X < IndexOffsetX; ++X)
        {
            for (int Y = -IndexOffsetY; Y < IndexOffsetY; ++Y)
            {
                GameObject CurrentCube = Instantiate(Cube, new Vector3(X, 0, Y), Quaternion.identity);
                CurrentCube.name = "Tile_" + X + "_" + Y;
                CurrentCube.transform.parent = transform;

                // TODO: remove after debug
                CurrentCube.GetComponent<Renderer>().material.SetColor("_Color", UnityEngine.Random.ColorHSV());

                GridTile.TileIndex CurrentIndex = new GridTile.TileIndex(X+IndexOffsetX, Y+IndexOffsetY);
                GridTile Tile = new GridTile(CurrentCube, CurrentIndex);

                Grid.Add(CurrentIndex, Tile);
            }
        }
        transform.Rotate(Vector3.up, GridRotation);
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
}
