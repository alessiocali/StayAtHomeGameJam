﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public GridMap GridMap { get; private set; }

    [SerializeField]
    protected List<GameObject> OccupantsToSpawnOnStart = new List<GameObject>();

    [SerializeField]
    public List<GameObject> PlayerCharacter = new List<GameObject>();

    private int CurrentOccupantToUpdateIdx = -1;

    private List<GameObject> RuntimeOccupantObjects = new List<GameObject>();
    private List<GameObject> PendingSpawnedObjectsToAdd = new List<GameObject>();

    private bool IsTurnActive = false;
    private bool IsGameDone = false;

    public void GameWon()
    {
        Debug.Log("Game Won :D");
        DisableHUD();
        IsGameDone = true;
        var go = GameObject.Find("WIN");
        var goCanvas = go.GetComponent<Canvas>();
        goCanvas.enabled = true;
    }

    public void GameOver()
    {
        Debug.Log("Game Over :(");
        DisableHUD();
        IsGameDone = true;
        var go = GameObject.Find("LOSE");
        var goCanvas = go.GetComponent<Canvas>();
        goCanvas.enabled = true;
    }

    public void SetToiletPaperHUDActive(bool active)
    {
        var go = GameObject.Find("Toilet Paper");
        var image = go.GetComponent<Image>();
        if (image != null)
        {
            image.enabled = active;
        }
    }

    public void SetCertificationHUDActive(bool active)
    {
        var go = GameObject.Find("Autocert");
        var image = go.GetComponent<Image>();
        if (image != null)
        {
            image.enabled = active;
        }
    }

    public void SetMaskHUDActive(bool active, int contagionLevel)
    {
        var go = GameObject.Find("Mask" + contagionLevel.ToString());
        if (go != null)
        {
            go.SetActive(active);
        }
    }

    public bool IsPlayerWaitingForInput()
    {
        return FindObjectOfType<Player>().IsWaitingForInput();
    }

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

        GameObject PrefabObject = Instantiate(prefab, tileCenter, prefab.transform.rotation);
        PrefabObject.GetComponent<Occupant>().CurrentTileIndex = index;

        PendingSpawnedObjectsToAdd.Add(PrefabObject);
    }

    private void DisableHUD()
    {
        SetToiletPaperHUDActive(false);
        SetCertificationHUDActive(false);
        SetMaskHUDActive(false, 1);
        SetMaskHUDActive(false, 2);
        SetMaskHUDActive(false, 3);
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
        SpawnOccupantObjectOnTile(PlayerCharacter[SharedSceneParameter.CharacterSelected], GridMap.GetHomeTileIndex());
        
        foreach (GameObject objectToSpawn in OccupantsToSpawnOnStart)
        {
            //Get tile without occupants and that's walkable
            SpawnOccupantObjectOnTile(objectToSpawn, GridMap.GetRandomTileIndex(false,true));
        }
    }

    private void UpdateTurn()
    {
        if (CurrentOccupantToUpdateIdx < 0)
        {
            CleanupDestroyedOccupants();
            CurrentOccupantToUpdateIdx = 0;
        }

        GameObject currentOccupant = RuntimeOccupantObjects[CurrentOccupantToUpdateIdx];
        StartCoroutine("ExecuteTurn", currentOccupant);
    }

    public IEnumerator ExecuteTurn(GameObject occupantToUpdate)
    {
        IsTurnActive = true;

        if (occupantToUpdate != null)
        {
            Occupant occupantComponentToUpdate = occupantToUpdate.GetComponent<Occupant>();
            yield return occupantComponentToUpdate.UpdateTurn();
        }

        IsTurnActive = false;
        UpdateRuntimeOccupants();
        CurrentOccupantToUpdateIdx++;

        if (CurrentOccupantToUpdateIdx == RuntimeOccupantObjects.Count)
        {
            CurrentOccupantToUpdateIdx = -1;
        }
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
        if (!IsGameDone && !IsTurnActive)
        {
            UpdateTurn();
        }
    }
}
