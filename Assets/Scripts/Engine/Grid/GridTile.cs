using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
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

    [SerializeField]
    public bool isWalkable;
    [SerializeField]
    public bool isBuilding;

    private void OnMouseOver()
    {
        
        if (!GameManager.Instance.IsPlayerWaitingForInput())
        {
            return;
        }

        if (isWalkable || isBuilding)
        {
            var tilesAround = GameManager.Instance.GridMap.GetCardinalTilesAround(Index);
            foreach(var tile in tilesAround)
            {
                if (tile.HasPlayerOccupant())
                {
                    SetGlowing(true);
                    return;
                }
            }            
        }
       
    }

    public void SetGlowing(bool glowing)
    {
        string shaderName = glowing ? "Custom/Glow" : "Standard";

        GetComponent<Renderer>().material.shader = Shader.Find(shaderName);
        if (transform.childCount > 0)
        {
            foreach (Renderer render in GetComponentsInChildren<Renderer>(false))
            {
                render.material.shader = Shader.Find(shaderName);
            }
        }
    }

    private void OnMouseExit()
    {
        SetGlowing(false);
    }

    private void UpdateBuildingGlowing()
    {
        if (!isBuilding)
        {
            return;
        }

        SetGlowing(Occupants.Count > 0);
    }

    public TileIndex Index { get; private set; }
    private Vector3 WordPosition;

    public GameObject TileObject { get; private set; } = null;

    private List<Occupant> Occupants = new List<Occupant>();
    
    public bool HasOccupants()
    {
        return Occupants.Count > 0;
    }

    public bool HasCharacterOccupant()
    {
        return Occupants.Find(occupant => occupant is Character) != null;
    }

    public bool HasPlayerOccupant()
    {
        return Occupants.Find(occupant => occupant is Player) != null;
    }

    public Player GetPlayerOccupant()
    {
        Occupant playerOccupant = Occupants.Find(occupant => occupant is Player);
        return playerOccupant != null ? playerOccupant as Player : null;
    }

    public void AddOccupant(Occupant occupant)
    {
        if (!Occupants.Contains(occupant))
        {
            Occupants.Add(occupant);
            UpdateBuildingGlowing();
        }
    }

    public void RemoveOccupant(Occupant occupant)
    {
        Occupants.Remove(occupant);
        UpdateBuildingGlowing();
    }

    public void OnOccupantCollided(Occupant other)
    {
        foreach (Occupant occupant in Occupants)
        {
            occupant.OnOtherOccupantCollided(other);
        }
    }

    public void InitializeGridTile (GameObject InTileObject, TileIndex InIndex)
    {
        TileObject= InTileObject;
        WordPosition = InTileObject.transform.position + new Vector3(0.5f, 1f, 0.5f);
        Index = InIndex;
    }

    public Vector3 GetOccupantPosition()
    {
        return WordPosition;
    }

    private void Update()
    {
        if (isBuilding && HasPlayerOccupant())
        {
            SetGlowing(true);
        }
    }
}
