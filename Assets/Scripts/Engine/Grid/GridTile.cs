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
        
        if (isWalkable)
        {
            var tilesAround = GameManager.Instance.GridMap.GetCardinalAndWalkableTilesAround(Index);
            foreach(var tile in tilesAround){
                if (tile.HasPlayerOccupant())
                {
                    if(GameManager.Instance.IsPlayerWaitingForInput())
                         GetComponent<Renderer>().material.shader = Shader.Find("Custom/Glow");
                }
            }            
        }
       
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.shader = Shader.Find("Standard");
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

    public void AddOccupant(Occupant occupant)
    {
        if (!Occupants.Contains(occupant))
        {
            Occupants.Add(occupant);
        }
    }

    public void RemoveOccupant(Occupant occupant)
    {
        Occupants.Remove(occupant);
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
}
