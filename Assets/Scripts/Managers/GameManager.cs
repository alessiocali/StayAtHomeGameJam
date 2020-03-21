using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public GridMap GridMap { get; private set; }

    [SerializeField]
    protected List<GameObject> OccupantsToSpawnOnStart = new List<GameObject>();

    private int CurrentOccupantToUpdateIdx = -1;

    private List<GameObject> RuntimeOccupantObjects = new List<GameObject>();
    private List<GameObject> PendingSpawnedObjectsToAdd = new List<GameObject>();

    /// <summary>
    /// Spawns on request the given prefab object to the target Tile
    /// </summary>
    /// <param name="prefab">The Prefab to Spawn</param>
    /// <param name="index">The index of the Tile where to Spawn the prefab</param>
    public void SpawnOccupantObjectOnTile(GameObject prefab, GridTile.TileIndex index)
    {
        // TODO Test for the cell not being occupied

        GridTile tile = GridMap.GetTileAt(index);
        Vector3 tileCenter = tile.GetOccupantPosition();
        PendingSpawnedObjectsToAdd.Add(Instantiate(prefab, tileCenter, Quaternion.identity));
    }

    private void CleanupDestroyedOccupants()
    {
        RuntimeOccupantObjects.RemoveAll(go => go == null);
    }

    private void UpdateRuntimeOccupants()
    {
        RuntimeOccupantObjects.AddRange(PendingSpawnedObjectsToAdd);
        PendingSpawnedObjectsToAdd.Clear();
    }

    private void SpawnStartingOccupants()
    {
        foreach (GameObject objectToSpawn in OccupantsToSpawnOnStart)
        {
            SpawnOccupantObjectOnTile(objectToSpawn, new GridTile.TileIndex(0, 0));
        }
    }

    private void UpdateTurn()
    {
        if (CurrentOccupantToUpdateIdx < 0)
        {
            CleanupDestroyedOccupants();
            CurrentOccupantToUpdateIdx = 0;
        }

        while (CurrentOccupantToUpdateIdx != RuntimeOccupantObjects.Count)
        {
            GameObject currentOccupant = RuntimeOccupantObjects[CurrentOccupantToUpdateIdx];
            if (currentOccupant != null)
            {
                Occupant.UpdateTurnResult result = currentOccupant.GetComponent<Occupant>().UpdateTurn();

                if (result == Occupant.UpdateTurnResult.Pending)
                {
                    return;
                }
            }

            UpdateRuntimeOccupants();
            CurrentOccupantToUpdateIdx++;
        }

        // Global turn has ended
        CurrentOccupantToUpdateIdx = -1;
    }

    private void Awake()
    {
        GridMap = FindObjectOfType<GridMap>();
        GridMap.GenerateGrid();
        SpawnStartingOccupants();
        UpdateRuntimeOccupants();
    }

    void Update()
    {
        UpdateTurn();
    }
}
