using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Dictionary<GridTile.TileIndex, GridTile> Grid;

    public List<GridTile> GetTilesAround(GridTile.TileIndex tileIndex)
    {
        return new List<GridTile>();
    }

    private List<Occupant> GetAllOccupants()
    {
        return new List<Occupant>();
    }

    private void UpdateAllOccupants()
    {

    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAllOccupants();
    }
}
