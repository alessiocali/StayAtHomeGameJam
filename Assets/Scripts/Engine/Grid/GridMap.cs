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
    public List<GameObject> NotWalkableCube = new List<GameObject>();
    public List<GameObject> WalkableCube = new List<GameObject>();


    [SerializeField]
    public float GridRotation = -45f;

    private Dictionary<GridTile.TileIndex, GridTile> Grid = new Dictionary<GridTile.TileIndex, GridTile>();

    public void GenerateGrid ()
    {
        if (WalkableCube.Count == 0 || NotWalkableCube.Count == 0)
        {
            return;
        }

        int IndexOffsetX = GRID_WIDTH / 2;
        int IndexOffsetY = GRID_HEIGHT / 2;

        // Generate a map around a center
        GameObject Originator = Instantiate(WalkableCube[0], new Vector3(0, 0, 0), Quaternion.Euler(0, GridRotation, 0), transform);
        for (int X = -IndexOffsetX; X < IndexOffsetX; ++X)
        {

            for (int Y = -IndexOffsetY; Y < IndexOffsetY; ++Y)
            {
                Vector3 NextVector = Originator.transform.forward * X + Originator.transform.right * Y;
                GameObject Current3Cube = Instantiate(WalkableCube[0], NextVector, Quaternion.Euler(0, GridRotation, 0), transform);
                Current3Cube.name = "Tile_" + (X + IndexOffsetX) + "_" + (Y + IndexOffsetY);

                GridTile.TileIndex CurrentIndex = new GridTile.TileIndex(X + IndexOffsetX, Y + IndexOffsetY);
                GridTile Tile = new GridTile(Current3Cube, CurrentIndex);
                Tile.isWalkable = true;
                Grid.Add(CurrentIndex, Tile);
            }
        }
        //GenerateRandomMap(Originator);

        GameObject.Destroy(Originator);
    }

    public struct SpawnZone
    {
        public int Xmin, Xmax;
        public int Ymin, Ymax;

        public SpawnZone (int xmn, int xmx, int ymn, int ymx)
        {
            Xmin = xmn;
            Ymin = ymn;
            Xmax = xmx;
            Ymax = ymx;
        }
    }

    public void GenerateRandomMap (GameObject originator)
    {
        if (!CheckExistsEssentialTiles())
        {
            return;
        }

        List<List<int>> Matrix = new List<List<int>>(GRID_WIDTH * GRID_HEIGHT);

        // Get house & supermarket
        GameObject HouseTile = NotWalkableCube.Where(go => go.tag.Equals("House")).First();
        GameObject SupermarketTile = NotWalkableCube.Where(go => go.tag.Equals("SuperMarket")).First();

        //Get random house location
        GridTile.TileIndex index = GetRandomTileIndex();

        //Get random supermarket location (min 4 tile)
        int GetMinXDist = (index.X - GRID_WIDTH/2) >= 0 ? (index.X - GRID_WIDTH/2) : 0;
        int GetMinYDist = (index.Y - GRID_HEIGHT/2) >= 0 ? (index.Y - GRID_HEIGHT/2) : 0;
        
        int GetMaxXDist = (index.X + GRID_WIDTH / 2) < GRID_WIDTH ? (index.X + GRID_WIDTH / 2) : GRID_WIDTH;
        int GetMaxYDist = (index.Y + GRID_HEIGHT / 2) < GRID_HEIGHT ? (index.Y + GRID_HEIGHT / 2) : GRID_HEIGHT;

        int MinXMarket = Mathf.Max(0, GetMinXDist);
        int MaxXMarket = Mathf.Min(GetMaxXDist, GRID_WIDTH);
        
        int MinYMarket = Mathf.Max(0, GetMinYDist);
        int MaxYMarket = Mathf.Min(GetMaxYDist, GRID_HEIGHT);

        // Two zone to spawn the supermarket
        List<SpawnZone> SpawnZones = new List<SpawnZone>();
        if (MinXMarket == 0) {
            SpawnZone sp = new SpawnZone();
            sp.Xmin = MaxXMarket;
            sp.Xmax = GRID_WIDTH;
            sp.Ymin = 0;
            sp.Ymax = GRID_HEIGHT;

            SpawnZones.Add(sp);
        }
        else if (MaxXMarket == GRID_WIDTH)
        {
            SpawnZone sp = new SpawnZone();
            sp.Xmin = 0;
            sp.Xmax = MinXMarket;
            sp.Ymin = 0;
            sp.Ymax = GRID_HEIGHT;

            SpawnZones.Add(sp);
        }
        if (MinYMarket == 0)
        {
            SpawnZone sp = new SpawnZone();
            sp.Ymin = MaxYMarket;
            sp.Ymax = GRID_HEIGHT;
            sp.Xmin = 0;
            sp.Xmax = GRID_WIDTH;

            SpawnZones.Add(sp);
        }
        else if (MaxYMarket == GRID_HEIGHT)
        {
            SpawnZone sp = new SpawnZone();
            sp.Ymin = 0;
            sp.Ymax = MinYMarket;
            sp.Xmin = 0;
            sp.Xmax = GRID_WIDTH;

            SpawnZones.Add(sp);
        }
        
        // Get Random zone
        int ZoneIndex = Random.Range(0, SpawnZones.Count);


        int MarketIndexX = Random.Range(SpawnZones[ZoneIndex].Xmin, SpawnZones[ZoneIndex].Xmax);
        int MarketIndexY = Random.Range(SpawnZones[ZoneIndex].Ymin, SpawnZones[ZoneIndex].Ymax);

        //int MarketIndexX = Random.Range(GetMaxXDist, GRID_WIDTH);
        //int MarketIndexY = Random.Range(GetMaxYDist, GRID_HEIGHT);

        Vector3 NextVector = originator.transform.forward * (index.X - GRID_WIDTH / 2) + originator.transform.right * (index.Y - GRID_HEIGHT / 2);
        GameObject Current3Cube = Instantiate(HouseTile, NextVector, Quaternion.Euler(0, GridRotation, 0), transform);

        NextVector = originator.transform.forward * (MarketIndexX - GRID_WIDTH / 2) + originator.transform.right * (MarketIndexY - GRID_HEIGHT / 2);
        Current3Cube = Instantiate(SupermarketTile, NextVector, Quaternion.Euler(0, GridRotation, 0), transform);

        //Rotate house  or buiding toward the map if is faci
        
    }

    public bool CheckExistsEssentialTiles ()
    {
        List<string> Tags = new List<string>();
        foreach (GameObject NonTile in NotWalkableCube)
        {
            Tags.Add(NonTile.tag);
        }

        if (Tags.Where(s => s.Equals("House") || s.Equals("SuperMarket")).Count() < 2)
        {
            return false;
        }
        Tags.Clear();

        foreach (GameObject Tile in WalkableCube)
        {
            Tags.Add(Tile.tag);
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
